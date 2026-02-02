# Talep Yönetim Sistemi (Request Management System)

Kurum içi kullanıcıların talep oluşturduğu, yöneticilerin talepleri onayladığı/reddettiği ve durum takibinin yapıldığı rol bazlı bir ASP.NET Core MVC uygulaması.

## Teknik Stack

- **ASP.NET Core 8 MVC** (.NET 8)
- **Entity Framework Core 8** (Code First)
- **MS SQL Server**
- **Razor Views**
- **Bootstrap 5**

## Roller ve Yetkiler

| Rol     | Yetkiler                                                                 |
|---------|--------------------------------------------------------------------------|
| User    | Talep oluşturur, düzenler, kendi taleplerini görüntüler                  |
| Manager | Tüm talepleri görür, onaylar / reddeder                                  |
| Admin   | Manager yetkileri + Kullanıcı Yönetimi                |

## Kurulum Adımları

1. **Gereksinimler**
   - .NET 8 SDK
   - SQL Server (LocalDB veya tam sürüm)
   - IDE: Visual Studio 2022 veya VS Code

2. **Projeyi klonlayın**
   ```bash
   git clone <repo-url>
   cd RequestManagementSystem
   ```

3. **Bağlantı dizesini ayarlayın**
   - `RequestManagementSystem/appsettings.json` içinde `ConnectionStrings:DefaultConnection` değerini kendi SQL Server bağlantınıza göre güncelleyin.
   - Örnek (LocalDB):
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=RequestManagementDb;Trusted_Connection=True;MultipleActiveResultSets=true"
   }
   ```

4. **Veritabanı ve migration**
   ```bash
   cd RequestManagementSystem
   dotnet ef database update
   ```
   (Migration yoksa: `dotnet ef migrations add InitialCreate` ardından `dotnet ef database update`)

5. **Uygulamayı çalıştırın**
   ```bash
   dotnet run
   ```
   Tarayıcıda varsayılan olarak: `https://localhost:7xxx` veya `http://localhost:7xxx`

6. **Test kullanıcıları (Seed data)**
   - **admin** / 123 — Admin
   - **manager** / 123 — Manager (onay/red)
   - **user** / 123 — User (talep oluşturma)

## Mimari Yaklaşım

- **MVC**: Controller–View–ViewModel ayrımı; talep listesi/detay/onay için ViewModel kullanımı.
- **Code First**: Entity’ler ve `AppDbContext` ile migration’lar üzerinden veritabanı yönetimi.
- **Rol bazlı erişim**: `RoleAuthorizeAttribute` ile controller/action seviyesinde yetki; Session’da UserId, Role, FullName tutuluyor.
- **Enum**: Talep türü, öncelik ve durum için enum kullanımı (RequestType, RequestPriority, RequestStatus, Role).
- **Talep geçmişi**: Onay/red işlemleri `RequestHistory` tablosuna kaydediliyor.

## Ana Modüller

1. **Kimlik doğrulama** — Login/Logout, Session, rol bazlı sayfa erişimi.
2. **Talep yönetimi** — Talep CRUD, otomatik talep no (TAL-YYYY-0001), taslak → onaya gönder → onay/red.
3. **Onay & durum** — Yönetici onayla/reddet (redde açıklama zorunlu), RequestHistory.
4. **Listeleme & filtreleme** — Tarih/durum filtreleri, başlık arama, sayfalama (paging).
5. **Dashboard** — Yönetici: toplam talep, onay bekleyen sayısı, son 5 talep; Kullanıcı: kendi talepleri ve son 5 talep.

## Proje Yapısı (Özet)

- `Controllers/` — Account, Request, Approval, Dashboard, User
- `Models/Entities/` — User, Request, RequestHistory
- `Models/ViewModels/` — Login, Request CRUD, Approval, Dashboard
- `Models/Enums/` — RequestType, RequestPriority, RequestStatus, Role
- `Data/` — AppDbContext, SeedData
- `Filters/` — RoleAuthorizeAttribute
- `Helpers/` — SessionHelper
- `Views/` — Account, Request, Approval, Dashboard, Shared

