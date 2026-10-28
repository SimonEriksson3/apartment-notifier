using ApartmentNotifier.Sources;
using Microsoft.Playwright;

namespace ApartmentNotifier.Services
{
    public class BrowserService
    {
        private IPlaywright _playwright;
        private IBrowser _browser;
        private IBrowserContext _context;
        //private IPage _page; //Page instance

        public async Task InitAsync()
        {
            // Initialize Playwright and launch the browser
            _playwright = await Playwright.CreateAsync();
            _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = false
            });
            _context = await _browser.NewContextAsync(new BrowserNewContextOptions
            {
                ViewportSize = new ViewportSize { Width = 1280, Height = 1024 }
            });
        }

        public async Task DeclineCookies(IPage page, string keyword)
        {
            var decline = page.GetByText(keyword, new() { Exact = false }); 

            if (await decline.CountAsync() > 0)
            {
                await decline.First.ClickAsync();
            }
        }

        public async Task GoToAsync(IPage page, string url)
        {
            await page.GotoAsync(url);
            await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        }

        /// <summary>
        /// Assigns unique tab (page) to each source
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task<IPage> CreatePageAsync()
        {
            if (_context == null)
            {
                throw new InvalidOperationException("Browser context is not initialized. Call Init() first.");
            }

            return await _context.NewPageAsync();
        }
    }
}
