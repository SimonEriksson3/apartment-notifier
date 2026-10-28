# Apartment Notifier 🌆

⚠️ **Work in progress**: *This project is in a very early stage. I am currently building the basic structure, so the code is not ready to be used yet.*

## What is this?
Apartment Notifier is an automated tool built with C# and .NET using the Playwright library.
The goal is to automatically check landlord websites for available apartments that match your filters. When new apartments show up, the program will send you a notification.

Instead of manually checking landlord websites every day, this tool does the work for you in the background.

## Tech Stack
* **Language:** C# (.NET)
* **Web Scraping:** [Microsoft.Playwright](https://playwright.dev/dotnet/)

## How it works
The project is structured to make it easy to add support for more websites, property types and notification services in the future.
* **Models:** Data classes, like `Property` and `Apartment`.
* **Services:** Handles tools like the browser instance (`BrowserService`).
* **Sources:** This is where the web scraping actually happens. All supported websites should use the `IApartmentSource` interface.
* **Manager:** `SearchManager` runs the main program loop.

## Roadmap
- [x] Set up core project architecture and data models
- [x] Integrate browser automation for web scraping
- [ ] 🚧**In progress:** Develop the initial scraper source (Balder)
- [ ] Extract apartment data from search results
- [ ] Add a local database to track previously seen apartments
- [ ] Implement at least one notification service (Email, Discord, etc.)
