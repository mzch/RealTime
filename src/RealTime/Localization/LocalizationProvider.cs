﻿// <copyright file="LocalizationProvider.cs" company="dymanoid">
// Copyright (c) dymanoid. All rights reserved.
// </copyright>

namespace RealTime.Localization
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Xml;
    using ColossalFramework.Globalization;
    using static Constants;

    internal sealed class LocalizationProvider
    {
        private readonly string localeStorage;
        private readonly Dictionary<string, string> translation = new Dictionary<string, string>();

        public LocalizationProvider(string rootPath)
        {
            localeStorage = Path.Combine(rootPath, LocaleFolder);
        }

        public string CurrentLanguage { get; private set; } = "en";

        public CultureInfo CurrentCulture { get; private set; } = CultureInfo.CurrentCulture;

        public string Translate(string id)
        {
            if (translation.TryGetValue(id, out string value))
            {
                return value;
            }

            return NoLocale;
        }

        public void LoadTranslation(string language)
        {
            if (!Load(language))
            {
                Load("en");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase", Justification = "No security issues here")]
        private static string GetLocaleNameFromLanguage(string language)
        {
            switch (language.ToLowerInvariant())
            {
                case "de":
                    return "de-DE";
                case "es":
                    return "es-ES";
                case "fr":
                    return "fr-FR";
                case "ko":
                    return "ko-KR";
                case "pl":
                    return "pl-PL";
                case "pt":
                    return "pt-PT";
                case "ru":
                    return "ru-RU";
                case "zh":
                    return "zh-CN";
                default:
                    return "en-US";
            }
        }

        private bool Load(string language)
        {
            translation.Clear();

            string path = Path.Combine(localeStorage, language + FileExtension);
            if (!File.Exists(path))
            {
                return false;
            }

            try
            {
                var doc = new XmlDocument();
                doc.Load(path);

                foreach (XmlNode node in doc.DocumentElement.ChildNodes)
                {
                    translation[node.Attributes[XmlKeyAttribute].Value] = node.Attributes[XmlValueAttribute].Value;
                }

                try
                {
                    CurrentCulture = new CultureInfo(GetLocaleNameFromLanguage(language));
                }
                catch
                {
                    CurrentCulture = LocaleManager.cultureInfo;
                }
            }
            catch
            {
                translation.Clear();
                return false;
            }

            CurrentLanguage = language;
            return true;
        }
    }
}