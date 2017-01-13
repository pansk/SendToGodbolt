//------------------------------------------------------------------------------
// <copyright file="SendToGodbolt.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.ComponentModel.Design;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace SendToGodBolt
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class SendToGodbolt
    {
        /// <summary>
        /// Command ID.
        /// </summary>
        public const int CommandId = 0x0100;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("522c9ea1-5f38-499a-b1ad-1eab054a873f");

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly Package package;

        /// <summary>
        /// Initializes a new instance of the <see cref="SendToGodbolt"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        private SendToGodbolt(Package package)
        {
            if (package == null)
            {
                throw new ArgumentNullException("package");
            }

            this.package = package;

            OleMenuCommandService commandService = this.ServiceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (commandService != null)
            {
                var menuCommandID = new CommandID(CommandSet, CommandId);
                var menuItem = new MenuCommand(this.MenuItemCallback, menuCommandID);
                commandService.AddCommand(menuItem);
            }
        }

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static SendToGodbolt Instance
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the service provider from the owner package.
        /// </summary>
        private IServiceProvider ServiceProvider
        {
            get
            {
                return this.package;
            }
        }

        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        public static void Initialize(Package package)
        {
            Instance = new SendToGodbolt(package);
        }

        /// <summary>
        /// This function is the callback used to execute the command when the menu item is clicked.
        /// See the constructor to see how the menu item is associated with this function using
        /// OleMenuCommandService service and MenuCommand class.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event args.</param>
        private void MenuItemCallback(object sender, EventArgs e)
        {
            var dte = ServiceProvider.GetService(typeof(SDTE)) as DTE;

            if (dte == null) return;

            var doc = dte.ActiveWindow.Document;

            if (doc == null) return;

            var extension = Path.GetExtension(doc.FullName);

            string[] validExtensions = { ".cc", ".cpp", ".cxx", ".c" };

            if (!validExtensions.Any(x => x.Equals(extension, StringComparison.InvariantCultureIgnoreCase))) return;

            dynamic selection = doc.Selection;

            string s = selection.Text;

            VsShellUtilities.OpenBrowser(
                $@"http://godbolt.org/#g:!((g:!((g:!((h:codeEditor,i:(j:1,options:(colouriseAsm:'0',compileOnChange:'0'),source:'{HttpUtility.UrlEncode(s)}'),l:'5',n:'1',o:'C%2B%2B+source+%231',t:'0')),k:24.449901768172893,l:'4',n:'0',o:'',s:0,t:'0'),(g:!((h:compiler,i:(compiler:clang391,filters:(b:'0',commentOnly:'0',directives:'0',intel:'0'),options:'-Ofast+-march%3Dnative+-std%3Dc%2B%2B14'),l:'5',n:'0',o:'%231+with+x86-64+clang+3.9.1',t:'0')),k:25.550098231827117,l:'4',n:'0',o:'',s:0,t:'0'),(g:!((h:compiler,i:(compiler:g63,filters:(b:'0',commentOnly:'0',directives:'0',intel:'0'),options:'-Ofast+-march%3Dnative+-std%3Dc%2B%2B14'),l:'5',n:'0',o:'%231+with+x86-64+gcc+6.3',t:'0')),k:25,l:'4',n:'0',o:'',s:0,t:'0'),(g:!((h:compiler,i:(compiler:cl19_32,filters:(b:'0',commentOnly:'0',directives:'0',intel:'0'),options:''),l:'5',n:'0',o:'%231+with+x86+CL+19+RC',t:'0')),k:25,l:'4',n:'0',o:'',s:0,t:'0')),l:'2',n:'0',o:'',t:'0')),version:4");

        }
    }
}
