using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PhilClipHelper
{
    class DialogFormat
    {
        // TODO: These format arrays are to be extended over time...
        public static readonly DialogFormat[] VideoFormats =
        {
            new DialogFormat("MPEG-4 Video", "*.mp4"),
            new DialogFormat("Flash Video", "*.flv"),
            new DialogFormat("Audio Video Interleave", "*.avi"),
            new DialogFormat("Matroska Video", "*.mkv"),
            new DialogFormat("QuickTime Video", "*.mov"),
        };

        public static readonly DialogFormat[] AudioFormats =
        {
            new DialogFormat("MPEG-4 Audio", "*.m4a"),
            new DialogFormat("MPEG-3 Audio", "*.mp3"),
            new DialogFormat("Waveform Audio", "*.wav"),
        };

        private string _description;
        public string Description
        {
            get => _description;
            set => _description = value;
        }

        private string _pattern;
        public string Pattern
        {
            get => _pattern;
            set => _pattern = value;
        }

        public DialogFormat(string description, string pattern)
        {
            _description = description;
            _pattern = pattern;
        }

        // Appends filters/formats for the Save/OpenFileDialog - only for internal use by SetOpen/SaveDialogFilters
        private static void AppendFileDialogFilters(FileDialog dialog, DialogFormat[] formats)
        {
            bool firstFormat = true;

            // We immediately want to add a '|', if this filter isn't empty, as we're about to append more formats to it
            if (!String.IsNullOrEmpty(dialog.Filter))
            {
                firstFormat = false;
            }

            // Can't modify fd.Filter directly, otherwise exceptions will be thrown - make a temporary variable first, and then copy it over to the file dialog filters
            string filter = "";
            foreach (DialogFormat format in formats)
            {
                if (!firstFormat)
                {
                    filter += "|";
                }

                firstFormat = false;
                filter += format._description + "|" + format._pattern;
            }

            // Note that this function is "Append", not "Set" - the caller has the responsibility to clear the filters beforehand
            dialog.Filter += filter;
        }

        // First adds a filter for all supported formats, then appends the rest of the filters/formats, and finally adds a filter for "All Files"
        public static void SetOpenDialogFilters(OpenFileDialog openDialog, DialogFormat[] formats)
        {
            bool firstFormat = true;
            string supportedExts = "";

            foreach (DialogFormat format in formats)
            {
                if (!firstFormat)
                {
                    supportedExts += ";";
                }

                firstFormat = false;
                supportedExts += format._pattern;
            }

            openDialog.Filter = "Supported files|" + supportedExts;
            AppendFileDialogFilters(openDialog, formats);
            openDialog.Filter += "|All files|*.*";
        }

        // Simply appends the listed formats for the save dialog filters, as we can't save as "Supported files"/"All files"
        public static void SetSaveDialogFilters(SaveFileDialog saveDialog, DialogFormat[] formats)
        {
            saveDialog.Filter = "";
            AppendFileDialogFilters(saveDialog, formats);
        }
    }
}

