🚀 Enterprise Stock Management & Hybrid Cloud Sync System
This project is a high-security, test-driven, hybrid stock management ecosystem that provides Real-time Synchronization between local and Azure Cloud databases using JWT & OAuth 2.0.

🏗️ Solution Structure
The project follows Clean Architecture principles and consists of 7 main projects:

StockManagement.API: Central management, JWT generation, and the AzureSyncWorker engine.

StockManagement.WebUI: OAuth 2.0 (Google Login) integration and user interface.

StockManagement.Business: Business logic, FluentValidation, and asynchronous service management.

StockManagement.DataAccess: Multi-DB Context management and EF Core Interceptor architecture.

StockManagement.Domain: ASP.NET Core Identity entities and database schemas.

StockManagement.Shared: Common helpers, DTOs, and cross-cutting configurations.

StockManagement.Tests: Unit and Integration tests powered by the Moq library.

🔐 Authentication & Identity
The system offers a corporate-level, flexible, and secure access management:

ASP.NET Core Identity: Comprehensive user management, role definitions, and secure password hashing.

JWT (JSON Web Token): Stateless authentication at the API layer. All requests are validated via claims-based signed tokens.

OAuth 2.0 & External Providers: "One-click login" integration with Google. Data from external providers is automatically mapped to internal Identity tables.

🔄 AzureSync & Outbox Design
Ensuring data consistency via an "Eventual Consistency" model:

Outbox Pattern: Operations are written to the main table while simultaneously being added to the OutboxEvents table as an atomic unit via EF Core Interceptor.

AzureSyncWorker: A background service that periodically scans local records and synchronizes them asynchronously to Azure SQL. This prevents Cloud latency from affecting API response times.

🛡️ Security Hardening (Defense in Depth)
XSS Protection: All user inputs are sanitized using HtmlSanitizer.

IDOR Protection: Operations are authorized via UserId extracted directly from JWT Claims rather than insecure QueryStrings.

Image Security: File uploads are validated through both extension and Magic Number (Header) checks to prevent malicious file execution.

Rate Limiting: Protection against Brute-force and DoS attacks via request throttling.

📡 Real-time & Desktop Integration (Infrastructure Ready)
SignalR Infrastructure: The core logic for real-time stock updates and messaging is implemented and ready for Web/Desktop synchronization.

Secure Desktop Handshake: The system is pre-configured in Program.cs and appsettings.json with Secret Keys for secure authentication between the API and future Desktop clients.

Encryption Service: A dedicated security service is integrated to handle asymmetric encryption for sensitive messaging data.

📊 Observability & Monitoring
Exception Logger: All runtime errors are logged to the database with full StackTrace details.

Audit & Session Tracking: Historical tracking of user page navigation and session durations.

Serilog: Structured logs are stored in physical files located in wwwroot/Logs/ for persistent monitoring.

🧪 Testing & Deployment
Moq Testing: Critical business services and logic are 100% verified through isolated test scenarios using Moq.

Docker Compose: The entire hybrid infrastructure is containerized and can be launched with a single command: docker-compose up -d