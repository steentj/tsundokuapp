# Tsundoku — Product Requirements Document (PRD)

Date: 3. januar 2026

## 1. Overview
Tsundoku is a personal library app for tracking books the user has bought but not necessarily read yet (their “tsundoku” pile). The app focuses on quick capture, simple organization, and an Apple-style minimal UI.

## 2. Goals
- Let users register purchased books with required details in < 60 seconds per book.
- Provide a clear overview of owned books with sorting/filtering by status.
- Support cover images via photo library, file picker, and camera.
- (When possible) auto-fill book details and cover by ISBN using free and legal APIs.
- Work on iPhone, iPad, and Mac (Mac Catalyst).
- All UI text is bilingual (Danish + English) with automatic selection: Danish if system language is Danish; otherwise English.

## 3. Non-goals (for MVP)
- Cross-device sync (iCloud/backend). “Shared across platforms” means shared code and a consistent local data model; each device stores its own local library.
- Social features (sharing, following, etc.).
- Barcode scanning (can be added later; MVP supports manual ISBN entry).

## 4. Target Platforms
- iPhone (iOS 15+)
- iPad (iPadOS 15+)
- Mac (Mac Catalyst 15+)

## 5. Personas
- **Impulse Buyer**: buys books frequently; wants quick logging and a cover photo.
- **Planner/Reader**: tracks reading progress and wants start/finish dates.
- **Reviewer**: wants ratings and short notes.

## 6. Core User Journeys
1. **Add book (manual)**
	- Open app → Add → enter title/author/date bought/reason/status → save.
2. **Add book (ISBN lookup)**
	- Enter ISBN → fetch metadata + cover → confirm/edit → save.
3. **Add/Update cover**
	- Choose photo / pick image file / take photo → app resizes to max 320×480 → stores locally.
4. **Update reading progress**
	- Change status + optionally set start/finish dates.
5. **Browse library**
	- View list of books → tap for edit → swipe to delete.

## 7. Functional Requirements

### 7.1 Book Fields
The user can store the following information per book.

**Mandatory**
- Title
- Author
- Date bought
- Reason for buying the book
- Status (one of):
  - Bought
  - Started reading
  - Read
  - Given up on reading it

**Optional**
- Image (cover)
- ISBN
- Short description
- Date started reading
- Date finished reading
- Review
- Rating (1–5 stars)

### 7.2 Validation Rules
- Title, Author, DateBought, ReasonForBuying, Status are required.
- Rating is optional; if set it must be 1–5.
- If DateFinishedReading is set, DateStartedReading must be set.
- If both start and finish dates exist, finish date must be ≥ start date.

### 7.3 Library View
- Displays a list of books with at minimum: cover thumbnail (if available), title, author, status.
- Sorting: default by DateBought (descending), then Title.
- Deleting: swipe-to-delete.

### 7.4 Add/Edit View
- Single form to create or edit a book.
- Save action persists data locally.
- Cover image actions:
  - Choose from photo library
  - Choose file (folder/file picker)
  - Take photo with camera

### 7.5 Image Handling
- Any selected/captured/downloaded image is resized to fit within 320×480 while preserving aspect ratio.
- Images are stored locally (app data folder). The database stores the path.

### 7.6 Book Metadata Lookup (Free + Legal)
**Primary recommendation**: Google Books API (better coverage for Danish books), with Open Library as fallback.

**MVP implementation note**: Start with Open Library lookup by ISBN (no API key required), then optionally add Google Books via API key.

Lookup behavior:
1. Try ISBN lookup.
2. If no match, user continues with manual entry.
3. Cache results locally to reduce requests and improve performance.

## 8. UX / UI Requirements
- Simple, minimal style following Apple Human Interface Guidelines.
- List/detail editing patterns consistent with iOS/macOS.
- Accessibility: support Dynamic Type where possible, readable contrast, and screen reader labels for key controls.

## 9. Localization (Danish + English)
- All in-app strings must exist in English and Danish.
- Language selection rule:
  - If system UI language is Danish (`da`), show Danish.
  - Otherwise show English.
- Permission prompts (camera/photos) must be localized on Apple platforms.

## 10. Data & Storage
### 10.1 Storage Choice
- Use **SQLite** for the book catalog (supports filtering/sorting, future growth, and reliable updates).
- Store cover images as files under the app data directory.

### 10.2 Data Model
Entity: Book
- Id (int)
- Title (string, required)
- Author (string, required)
- DateBought (date, required)
- ReasonForBuying (string, required)
- Status (enum, required)
- ImagePath (string?)
- ISBN (string?)
- ShortDescription (string?)
- DateStartedReading (date?)
- DateFinishedReading (date?)
- Review (string?)
- Rating (int?, 1–5)
- UpdatedAtUtc (date)

## 11. Privacy & Permissions
- iOS/Mac Catalyst:
  - Camera usage description required.
  - Photo library usage description required.
  - Localize permission strings (English default + Danish override).
- Android (for completeness if later enabled): camera + media image permissions.

## 12. Milestones
### MVP (v0.1)
- Local SQLite storage
- Library list + add/edit form
- DA/EN localization framework

### v1
- Image picking + resizing
- Open Library ISBN lookup + cover download

### v1.1
- Optional Google Books integration (API key)
- Filtering/sorting UI

## 13. Risks / Open Questions
- Metadata coverage for Danish editions varies by source; ensure manual entry is always smooth.
- Google Books ToS and quotas: confirm compliance for App Store distribution.
- Cross-device sync is not in MVP; clarify expectations if needed.