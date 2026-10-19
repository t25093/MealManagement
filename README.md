# MealManagement – שרת

מערכת לתיאום ארוחות משותפות. בעלת ארוחה יוצרת ארוחה (חלבית או בשרית), קובעת מספר אורחים ובוחרת מאכלים לכל קטגוריה, והמערכת מחשבת אוטומטית כמה צריך להכין מכל מאכל לפי מספר האורחים. משתתפות שהוזמנו לפי אימייל רואות את הארוחה, לוקחות על עצמן מנות או חלק ממנה ("אני מכינה"), בוחרות מאכל למנה שלא הוגדר לה מאכל, מתכתבות בצ'אט של הארוחה ומשתפות מתכונים.

השרת בנוי כ-ASP.NET Core Web API (.NET 8) עם PostgreSQL 16 ו-Entity Framework Core. הלקוח (React) נמצא ב-repository נפרד: `meal-management-client`.

---

## המשאב המוגבל והטיפול בתחרות

**המשאב המוגבל** הוא הכמות שנותרה להכנה מכל מנה בארוחה (`MealDish`). **התחרות** מתרחשת כשכמה משתתפות לוקחות או מפצלות את אותה מנה בו-זמנית, או כשבעלת הארוחה משנה את מספר האורחים בזמן שמשתתפת לוקחת. **הטיפול:** Optimistic Concurrency עם עמודת המערכת `xmin` של PostgreSQL כ-concurrency token. כל עדכון נשמר רק אם השורה לא השתנתה מאז שנקראה. בהתנגשות השרת מחזיר `409 Conflict`, נכתב לוג Warning, והלקוח מציג הודעה ומרענן את הנתונים.

> "שתי משתתפות לחצו בו-זמנית 'אני מכינה' על 40 המנות האחרונות של הקוגל. שתיהן קיבלו אישור, ואסור היה ששתיהן יקבלו."

במערכת הזו רק אחת מהן מקבלת אישור (201). השנייה מקבלת 409 עם ההודעה "המנה עודכנה על ידי משתתפת אחרת. רענני את הנתונים ונסי שוב."

**למה `xmin`:** ב-PostgreSQL אין `rowversion`. העמודה `xmin` קיימת בכל טבלה ומתעדכנת אוטומטית בכל שינוי בשורה, ולכן אין צורך להוסיף עמודה או לנהל ערך בקוד. היא ממופה למאפיין `uint Version` על `MealDish` עם `IsRowVersion()` ב-Fluent API.

**איפה זה בקוד:**
| מה | איפה |
|---|---|
| הגדרת ה-token | `MealManagement.Data/Configurations/MealDishConfiguration.cs` |
| בדיקת הכמות ועדכון בזיכרון, על אותה ישות שנטענה עם מעקב | `MealManagement.Service/UserDishAssignmentService.cs` |
| תפיסת `DbUpdateConcurrencyException` בלבד והמרה ל-`ConflictException` | `MealManagement.Data/UnitOfWork.cs` |
| החזרת 409 ולוג Warning | `MealManagement.API/Middleware/ExceptionHandlingMiddleware.cs` |
| בדיקה עם שני מופעים של `AppDbContext` מול PostgreSQL אמיתי | `MealManagement.Tests/Concurrency/MealDishConcurrencyTests.cs` |

---

## מבנה ה-Solution

| פרויקט | תפקיד | מפנה אל |
|---|---|---|
| `MealManagement.Core` | ישויות, Enums, DTOs, ממשקים, חריגות דומיין והגדרות. בלי תלות בטכנולוגיה | – |
| `MealManagement.Data` | `AppDbContext`, Fluent API, Migrations, Seed, Repositories ו-UnitOfWork | Core |
| `MealManagement.Service` | לוגיקה עסקית, הרשאות ברמת משאב, AutoMapper ו-JWT | Core |
| `MealManagement.API` | Controllers, Middleware, Program.cs, Swagger ו-NLog | Core, Service, Data (רישום DI בלבד) |
| `MealManagement.Tests` | xUnit ו-Moq על ה-Services, ובדיקת concurrency | Core, Service, Data |

---

## הרצה מקומית

### דרישות
- **.NET 8 SDK**
- **PostgreSQL 16**, באחת משתי הדרכים:
  - התקנה מקומית מ-postgresql.org (כוללת את pgAdmin), או
  - קונטיינר:
    ```
    docker run --name mealmanagement-pg -e POSTGRES_PASSWORD=<your-password> -p 5432:5432 -d postgres:16
    ```
- תעודת פיתוח ל-HTTPS (פעם אחת במחשב):
  ```
  dotnet dev-certs https --trust
  ```

### 1. User Secrets
אין אף סוד ב-repository. לפני ההרצה הראשונה מגדירים שני מפתחות ב-User Secrets של פרויקט ה-API, מתיקיית ה-Solution:

```
dotnet user-secrets set "ConnectionStrings:Default" "Host=localhost;Port=5432;Database=MealManagement;Username=postgres;Password=<your-password>" --project MealManagement.API
dotnet user-secrets set "Jwt:Key" "<random-string-of-at-least-32-characters>" --project MealManagement.API
```

אפשר גם ב-Visual Studio: קליק ימני על `MealManagement.API` ← **Manage User Secrets**, ולכתוב:
```json
{
  "ConnectionStrings": {
    "Default": "Host=localhost;Port=5432;Database=MealManagement;Username=postgres;Password=<your-password>"
  },
  "Jwt": {
    "Key": "<random-string-of-at-least-32-characters>"
  }
}
```

אם אחד המפתחות חסר, או ש-`Jwt:Key` קצר מ-32 תווים, האפליקציה נעצרת כבר בעלייה עם הודעה ברורה (`ValidateOnStart`).

### 2. הרצה
ב-Visual Studio: לבחור את הפרופיל **https** וללחוץ F5. או משורת הפקודה:
```
dotnet run --project MealManagement.API --launch-profile https
```

**את בסיס הנתונים לא יוצרים ידנית.** בסביבת Development האפליקציה מריצה בעלייה את כל ה-Migrations (`InitialCreate`, `AddChatRecipesAndConcurrency`) ואת ה-Seed. בסיס הנתונים `MealManagement` נוצר אוטומטית אם אינו קיים.

### 3. Swagger
```
https://localhost:7012/swagger
```
מתחברים דרך `POST /api/auth/login`, מעתיקים את ה-`token` מהתשובה, לוחצים **Authorize** ומדביקים אותו.

בנוסף, הקובץ `MealManagement.API/MealManagement.API.http` מכיל את כל הבקשות לפי הסדר, עם הסטטוס הצפוי לכל אחת.

### 4. לוגים
NLog כותב לתיקייה `logs` שליד קובץ ההרצה (`bin/Debug/net8.0/logs`): קובץ יומי `app-<date>.log`, קובץ שגיאות `errors-<date>.log`, וגם ל-Console. כל שורה כוללת CorrelationId, שחוזר גם בכותרת `X-Correlation-ID` של התשובה. סיסמאות, tokens ותוכן של בקשות הרשמה והתחברות לא נכתבים ללוג.

---

## משתמשי דמו

נוצרים אוטומטית ב-Seed:

| Role | אימייל | סיסמה | מה אפשר להדגים |
|---|---|---|---|
| **Admin** | `admin@meals.local` | `Admin123!` | רשימת כל המשתמשות (`GET /api/users`) ומחיקה של משתמשת (`DELETE /api/users/{id}`), עריכה ומחיקה של מאכלים במאגר. חשבון של Admin לא נמחק. היא גם **בעלת** הארוחה לדוגמה "ארוחת ערב משפחתית" |
| **User** | `user@meals.local` | `User123!` | **משתתפת** בארוחה לדוגמה: לקיחת מנה, פיצול, צ'אט ומתכונים. מקבלת 403 על פעולות של Admin ושל בעלת הארוחה. יכולה למחוק את החשבון שלה (`DELETE /api/users/me`) |

בשרת ב-Render הסיסמה של `admin@meals.local` היא הערך של משתנה הסביבה `Admin__Password`, ולא `Admin123!`. השרת מעדכן אותה בכל עלייה.

בעלת ארוחה יכולה לתת למשתתפת **הרשאת עריכה** (`PUT /api/meals/{id}/editors/{userId}`, וביטול ב-`DELETE`). עורכת יכולה לשנות את פרטי הארוחה, להוסיף ולהסיר מנות ולהזמין משתתפות. מחיקת הארוחה, הסרת משתתפות וניהול ההרשאות נשארים אצל בעלת הארוחה.

כדי להדגים שתי משתתפות שמתחרות על אותה מנה, נרשמים עם משתמשת User נוספת (`POST /api/auth/register`, או "אין לי חשבון" בלקוח), ובעלת הארוחה מוסיפה אותה כמשתתפת לפי האימייל.

בנוסף, ב-Seed יש כ-20 מאכלים במאגר מכל הקטגוריות (חלבי, בשרי ופרווה).

---

## מיילים

המערכת שולחת מייל כשמצרפים משתתפת לארוחה (משתמשת רשומה מקבלת קישור לארוחה, ומי שלא רשומה מקבלת הזמנה להירשם), וכשמבקשים איפוס סיסמה ("שכחתי סיסמה" במסך ההתחברות). הקישור לאיפוס תקף לשעה אחת ומשמש פעם אחת.

השליחה עוברת דרך ה-API של [Brevo](https://www.brevo.com) ב-HTTPS, כי במסלול החינמי של Render שליחה רגילה (SMTP) חסומה. ההגדרות:

| משתנה | ערך |
|---|---|
| `Email__ApiKey` | מפתח ה-API מ-Brevo |
| `Email__SenderEmail` | כתובת שאושרה ב-Brevo ככתובת שולחת |

בלי `Email__ApiKey` (למשל במחשב המקומי) המיילים לא נשלחים, אלא נרשמים ביומן של השרת עם הקישורים שבהם. כך אפשר לבדוק איפוס סיסמה גם בלי Brevo: מעתיקים את הקישור מהיומן.

## השרת ב-Render נרדם

במסלול החינמי Render מרדים את השרת אחרי 15 דקות בלי בקשות, וההתעוררות לוקחת כדקה. שני דברים עוזרים:

- **`.github/workflows/keep-alive.yml`**: GitHub שולח כל 10 דקות בקשה ל-`/health`, כדי שהשרת לא יירדם. זה קורה רק בימים א'-ו', בערך מ-6:00 או 7:00 בבוקר עד 23:00. כך השרת לא גומר את 750 השעות החינמיות בחודש של Render. אפשר לעצור את זה בלשונית **Actions** ב-GitHub, או למחוק את הקובץ.
- **`GET /health`**: בדיקה קלה שהשרת ער, בלי התחברות ובלי בסיס נתונים. האתר שולח אותה ברגע שהוא נפתח, וכל בקשה שמתעכבת יותר מ-3 שניות מציגה באתר את ההודעה "בטעינה…".

## הרצת הבדיקות

```
dotnet test
```
או ב-Visual Studio: **Test** ← **Run All Tests**.

- **Unit Tests** (`Services/`): בודקים את שכבת ה-Service עם Moq, ואינם צריכים בסיס נתונים. ביניהם לקיחה תקינה, פיצול, לקיחה של יותר ממה שנותר (Conflict, ו-`SaveChanges` לא נקרא), התנגשות שעוברת הלאה, משתמשת שאינה משתתפת (Forbidden) וכשרות.
- **בדיקת ה-Concurrency** (`Concurrency/`): רצה מול **שרת PostgreSQL פעיל**, כי `xmin` קיימת רק ב-PostgreSQL. היא יוצרת בסיס נתונים ייעודי, פותחת שני מופעים נפרדים של `AppDbContext` שקוראים את אותו `MealDish`, ומוודאת שהשמירה הראשונה מצליחה והשנייה זורקת `DbUpdateConcurrencyException`. בסוף היא מוחקת את בסיס הבדיקות.

**הגדרת ה-connection string לבדיקת ה-Concurrency** (בסיס נפרד, לא בסיס הפיתוח):
```
dotnet user-secrets set "ConnectionStrings:TestDb" "Host=localhost;Port=5432;Database=MealManagement_Tests;Username=postgres;Password=<your-password>" --project MealManagement.Tests
```
או כמשתנה סביבה (`ConnectionStrings__TestDb`). בלי ההגדרה, רק בדיקת ה-Concurrency נכשלת עם הודעה שמסבירה מה חסר. ה-Unit Tests רצים בכל מקרה.

להרצת בדיקת ה-Concurrency בלבד:
```
dotnet test --filter "FullyQualifiedName~Concurrency"
```
