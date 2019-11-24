using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Plugin.Models;

namespace Plugin.Controllers
{

    [Route("/Plugin")]
    public class PluginController : Controller
    {
        private readonly PageViewModel _pageViewModel;
        private readonly ILogger<PluginController> _logger;

        public PluginController(ILogger<PluginController> logger)
        {
            _pageViewModel = new PageViewModel();
            _logger = logger;
        }

        [Route("/Plugin/Index")]
        public IActionResult Index()
        {
            var manifestResourceNames = GetType().Assembly.GetManifestResourceNames();

            var pluginJavaScriptStream = this.GetType().Assembly.GetManifestResourceStream("Plugin.wwwroot.js.plugin.js");

            string scripts = string.Empty;

            using (var reader = new StreamReader(pluginJavaScriptStream, Encoding.UTF8))
            {
                scripts += reader.ReadToEnd();
            }

            var pluginStyleStream = this.GetType().Assembly.GetManifestResourceStream("Plugin.wwwroot.css.plugin.css");

            string styles = string.Empty;

            using (var reader = new StreamReader(pluginStyleStream, Encoding.UTF8))
            {
                styles += reader.ReadToEnd();
            }

            _pageViewModel.Scripts = scripts;
            _pageViewModel.Styles = styles;

            return View(_pageViewModel);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
