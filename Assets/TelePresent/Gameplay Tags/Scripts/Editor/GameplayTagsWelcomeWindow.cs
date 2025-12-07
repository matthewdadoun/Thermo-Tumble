/*******************************************************
Product - Gameplay Tags
  Publisher - TelePresent Games
              http://TelePresentGames.dk
  Author    - Martin Hansen
  Created   - 2025
  (c) 2025 Martin Hansen. All rights reserved.
*******************************************************/

using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace TelePresent.GameplayTags
{
    [InitializeOnLoad]
    public class GameplayTagsWelcomeWindow : EditorWindow
    {
        private Texture2D docsIcon;
        private Texture2D discordIcon;
        private const string IconsFolderPath = "Assets/TelePresent/Gameplay Tags/Editor/";
        private const string DocsIconPath = IconsFolderPath + "docs.png";
        private const string DiscordIconPath = IconsFolderPath + "discord.png";

        private double animationTimer;
        private double lastTime;
        private const float AnimationSpeed = 2f;
        private Vector2 newsScrollPos;
        private Vector2 mainScrollPos; 
        private List<NewsItem> newsItems = new List<NewsItem>();
        private bool isLoadingNews = false;
        private string newsLoadError = "";

        // List of asset links for supporting your work.
        private List<AssetLink> otherAssets = new List<AssetLink>()
        {
            new AssetLink() { Title = "Audio Sync Pro", URL = "https://prf.hn/click/camref:1011l52ZZU/destination:https://assetstore.unity.com/packages/tools/audio/audio-sync-pro-294046" },
            new AssetLink() { Title = "Character Creator", URL = "https://prf.hn/click/camref:1011l52ZZU/destination:https://assetstore.unity.com/packages/tools/utilities/character-creator-framework-instant-customization-305742" },
            new AssetLink() { Title = "Smart Mouse", URL = "https://prf.hn/click/camref:1011l52ZZU/destination:https://assetstore.unity.com/packages/tools/utilities/smart-mouse-selection-tools-294764" },
            new AssetLink() { Title = "Sound Shapes", URL = "https://prf.hn/click/camref:1011l52ZZU/destination:https://assetstore.unity.com/packages/tools/audio/sound-shapes-dynamic-audio-areas-312038" },
            new AssetLink() { Title = "Animation Viewer", URL = "https://prf.hn/click/camref:1011l52ZZU/destination:https://assetstore.unity.com/packages/tools/animation/editor-animation-viewer-292289" },
            new AssetLink() { Title = "Leave a Review", URL = "https://u3d.as/3vwi" },
        };

        static GameplayTagsWelcomeWindow()
        {
            EditorApplication.update -= TriggerWelcomeScreen;
            EditorApplication.update += TriggerWelcomeScreen;
        }

        private static void TriggerWelcomeScreen()
        {
            if (GameplayTags_EditorStartupHelper.FirstInitialization)
            {
                GameplayTags_EditorStartupHelper.FirstInitialization = false;
                GameplayTags_EditorStartupHelper.PersistStartupPreferences();
                ShowWindow();
            }
            else if (GameplayTags_EditorStartupHelper.DisplayWelcomeOnLaunch && EditorApplication.timeSinceStartup < 30f)
            {
                ShowWindow();
            }

            EditorApplication.update -= TriggerWelcomeScreen;
        }

        private static bool ShowOnStartup
        {
            get => GameplayTags_EditorStartupHelper.DisplayWelcomeOnLaunch;
            set => GameplayTags_EditorStartupHelper.DisplayWelcomeOnLaunch = value;
        }
        public static void ShowWindow()
        {
            var window = GetWindow<GameplayTagsWelcomeWindow>("Welcome Window");
            window.minSize = new Vector2(500, 650);
            window.Show();
        }

        private void OnEnable()
        {
            docsIcon = LoadIcon(DocsIconPath);
            discordIcon = LoadIcon(DiscordIconPath);

            lastTime = EditorApplication.timeSinceStartup;
            animationTimer = 0f;
            EditorApplication.update += UpdateAnimation;

            FetchNews();
        }

        private void OnDisable()
        {
            EditorApplication.update -= UpdateAnimation;
        }

        private Texture2D LoadIcon(string path)
        {
            return AssetDatabase.LoadAssetAtPath<Texture2D>(path);
        }

        private async void FetchNews()
        {
            isLoadingNews = true;
            newsLoadError = "";
            Repaint();

            string url = "https://telepresentgames.dk/Unity%20Asset/Gameplay%20Tags/GameplayTags_News.txt";

            try
            {
                UnityWebRequest request = UnityWebRequest.Get(url);
                var operation = request.SendWebRequest();

                while (!operation.isDone)
                    await Task.Yield();

                if (request.result != UnityWebRequest.Result.Success)
                {
                    newsLoadError = "Encountered an error loading news, no biggie, though!";

                }
                else
                {
                    ParseNews(request.downloadHandler.text);
                }
            }
            catch (System.Exception ex)
            {
                newsLoadError = $"Encountered an error loading news, no biggie, though!: {ex.Message}";
            }

            isLoadingNews = false;
            Repaint();
        }

        private void ParseNews(string rawText)
        {
            newsItems.Clear();
            string[] separators = new string[] { "\n---\n", "\r\n---\r\n" };
            string[] entries = rawText.Split(separators, System.StringSplitOptions.RemoveEmptyEntries);

            foreach (var entry in entries)
            {
                string[] lines = entry.Split(new[] { '\n', '\r' }, System.StringSplitOptions.RemoveEmptyEntries);
                if (lines.Length >= 3)
                {
                    string dateLine = lines[0].Trim();
                    string headlineLine = lines[1].Trim();
                    string bodyText = string.Join("\n", lines, 2, lines.Length - 2).Trim();

                    if (dateLine.StartsWith("[") && dateLine.EndsWith("]"))
                    {
                        dateLine = dateLine.Substring(1, dateLine.Length - 2);
                    }

                    newsItems.Add(new NewsItem
                    {
                        Date = dateLine,
                        Headline = headlineLine,
                        Body = bodyText
                    });
                }
                else
                {
                    Debug.LogWarning("Invalid news entry format - Each entry should have a date, headline, and body.");
                }
            }
        }

        private void UpdateAnimation()
        {
            double currentTime = EditorApplication.timeSinceStartup;
            double deltaTime = currentTime - lastTime;
            lastTime = currentTime;
            animationTimer += deltaTime * AnimationSpeed;
            Repaint();
        }

        private void OnGUI()
        {
            // Wrap all content in a vertical scroll view.
            mainScrollPos = GUILayout.BeginScrollView(mainScrollPos);

            GUILayout.Space(25);
            DrawHeader();
            GUILayout.Space(0);
            DrawIntro();
            GUILayout.Space(5);
            DrawButtons();
            GUILayout.Space(15);
            DrawOtherAssets();  // New section for linking other assets.
            GUILayout.Space(15);
            DrawToggle();
            GUILayout.Space(15);
            DrawNews();
            GUILayout.FlexibleSpace();
            DrawFooter();

            GUILayout.EndScrollView();
        }

        private void DrawHeader()
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Thank you for choosing \nGameplay Tags! 😊", GetTitleStyle());
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        private void DrawIntro()
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label(
                "I hope this tool helps you improve your conditional logic in Unity!\n\nBelow, you'll find helpful resources and ways to reach out.",
                GetBodyStyle(),
                GUILayout.Width(position.width - 80)
            );
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        private void DrawButtons()
        {
            DrawCenteredButton(
                "Documentation",
                "Learn about features, setup, and best practices.",
                docsIcon,
                () => Application.OpenURL("https://telepresentgames.dk/Unity%20Asset/Gameplay%20Tags/Gameplay%20Tags%20Documentation.pdf")
            );
            DrawAnimatedDiscordButton();
        }

        private void DrawCenteredButton(string title, string description, Texture2D icon, System.Action onClick)
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.BeginVertical();
            GUILayout.Label(description, GetDescriptionStyle(), GUILayout.Width(350));
            GUILayout.Space(5);

            var content = icon != null ? new GUIContent($"  {title}", icon) : new GUIContent($"  {title}");
            if (GUILayout.Button(content, GetButtonStyle(), GUILayout.Width(350), GUILayout.Height(45)))
            {
                onClick?.Invoke();
            }

            GUILayout.EndVertical();
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(0);
        }

        private void DrawAnimatedDiscordButton()
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.BeginVertical();
            GUILayout.Label("Ask questions, troubleshoot, and share your work!", GetDescriptionStyle(), GUILayout.Width(350));
            GUILayout.Space(5);

            Color animatedColor = Color.Lerp(Color.white, new Color(1f, 0.5f, 0.5f), (Mathf.Sin((float)animationTimer) + 1f) / 2f);

            GUIStyle animatedButtonStyle = new GUIStyle(GetButtonStyle())
            {
                normal = { textColor = animatedColor },
                focused = { textColor = animatedColor },
                hover = { textColor = animatedColor },
                active = { textColor = animatedColor }
            };

            var content = discordIcon != null ? new GUIContent($"  Join Discord", discordIcon) : new GUIContent($"  Join Discord");
            if (GUILayout.Button(content, animatedButtonStyle, GUILayout.Width(350), GUILayout.Height(45)))
            {
                Application.OpenURL("https://discord.gg/DCWnPkRmTf");
            }

            GUILayout.EndVertical();
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(15);
        }
        private void DrawOtherAssets()
        {
            GUILayout.BeginVertical("box");
            GUILayout.Label("Support My Work", GetSectionHeaderStyle());
            GUILayout.Label("If you appreciate my work, please consider exploring my other assets.", GetBodyStyle());
            GUILayout.Space(5);

            if (otherAssets != null && otherAssets.Count > 0)
            {
                int assetCount = 0;
                while (assetCount < otherAssets.Count)
                {
                    // If it's the last asset, draw it solo on a new line
                    if (assetCount == otherAssets.Count - 1)
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.FlexibleSpace();

                        AssetLink asset = otherAssets[assetCount];
                        if (GUILayout.Button(asset.Title, GetButtonStyle(), GUILayout.Width(150), GUILayout.Height(40)))
                        {
                            Application.OpenURL(asset.URL);
                        }

                        GUILayout.FlexibleSpace();
                        GUILayout.EndHorizontal();
                        GUILayout.Space(10);
                        assetCount++;
                    }
                    else
                    {
                        // Draw a row of up to 2 buttons if the next one is the last
                        int itemsInRow = (assetCount + 2 >= otherAssets.Count) ? 2 : 3;

                        GUILayout.BeginHorizontal();
                        GUILayout.FlexibleSpace();
                        for (int i = 0; i < itemsInRow && assetCount < otherAssets.Count - 1; i++, assetCount++)
                        {
                            AssetLink asset = otherAssets[assetCount];
                            if (GUILayout.Button(asset.Title, GetButtonStyle(), GUILayout.Width(150), GUILayout.Height(40)))
                            {
                                Application.OpenURL(asset.URL);
                            }

                            if (i < itemsInRow - 1)
                            {
                                GUILayout.Space(10);
                            }
                        }
                        GUILayout.FlexibleSpace();
                        GUILayout.EndHorizontal();
                        GUILayout.Space(10);
                    }
                }
            }

            GUILayout.EndVertical();
        }


        private void DrawNews()
        {
            GUILayout.BeginVertical("box");
            GUILayout.Label("News", GetSectionHeaderStyle());

            if (isLoadingNews)
            {
                GUILayout.Label("Loading news...", GetLoadingStyle());
            }
            else if (!string.IsNullOrEmpty(newsLoadError))
            {
                GUILayout.Label(newsLoadError, GetErrorStyle());
            }
            else if (newsItems.Count == 0)
            {
                GUILayout.Label("No news available.", GetBodyStyle());
            }
            else
            {
                // Begin scroll view for news.
                newsScrollPos = GUILayout.BeginScrollView(newsScrollPos, GUILayout.Height(200));

                foreach (var news in newsItems)
                {
                    GUILayout.BeginVertical("box");

                    // Date and Headline
                    GUILayout.BeginHorizontal();
                    GUILayout.Label($"[{news.Date}]", GetNewsDateStyle(), GUILayout.Width(100));
                    GUILayout.Label(news.Headline, GetNewsHeadlineStyle());
                    GUILayout.EndHorizontal();

                    // Body Text
                    GUILayout.Label(news.Body, GetNewsBodyStyle());

                    GUILayout.EndVertical();
                    GUILayout.Space(10);
                }

                GUILayout.EndScrollView();
            }

            GUILayout.EndVertical();
        }

        private void DrawToggle()
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            ShowOnStartup = GUILayout.Toggle(ShowOnStartup, "Show window on start-up", GUILayout.Width(250));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        private void DrawFooter()
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("© 2025 TelePresent Games", EditorStyles.centeredGreyMiniLabel);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        private GUIStyle GetTitleStyle()
        {
            return new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = 22,
                alignment = TextAnchor.MiddleCenter,
                wordWrap = true,
                margin = new RectOffset(10, 10, 5, 10)
            };
        }

        private GUIStyle GetBodyStyle()
        {
            return new GUIStyle(EditorStyles.wordWrappedLabel)
            {
                fontSize = 14,
                alignment = TextAnchor.UpperCenter,
                wordWrap = true,
                margin = new RectOffset(10, 10, 5, 10)
            };
        }

        private GUIStyle GetDescriptionStyle()
        {
            return new GUIStyle(EditorStyles.wordWrappedLabel)
            {
                fontSize = 12,
                alignment = TextAnchor.MiddleLeft,
                wordWrap = true,
                padding = new RectOffset(0, 10, 5, 5),
                margin = new RectOffset(0, 0, 5, 5)
            };
        }

        private GUIStyle GetButtonStyle()
        {
            return new GUIStyle(GUI.skin.button)
            {
                fontSize = 13,
                alignment = TextAnchor.MiddleCenter,
                padding = new RectOffset(15, 15, 10, 10),
                fontStyle = FontStyle.Bold
            };
        } 

        private GUIStyle GetSectionHeaderStyle()
        {
            return new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = 16,
                alignment = TextAnchor.MiddleLeft,
                margin = new RectOffset(10, 10, 5, 10)
            };
        }

        private GUIStyle GetNewsDateStyle()
        {
            return new GUIStyle(EditorStyles.label)
            {
                fontSize = 12,
                alignment = TextAnchor.MiddleLeft,
                fontStyle = FontStyle.Italic,
                normal = { textColor = Color.gray },
                margin = new RectOffset(10, 10, 5, 5)
            };
        }

        private GUIStyle GetNewsHeadlineStyle()
        {
            return new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = 16,
                alignment = TextAnchor.MiddleLeft,
                wordWrap = true,
                margin = new RectOffset(0, 10, 5, 5)
            };
        }

        private GUIStyle GetNewsBodyStyle()
        {
            return new GUIStyle(EditorStyles.wordWrappedLabel)
            {
                fontSize = 14,
                alignment = TextAnchor.UpperLeft,
                wordWrap = true,
                margin = new RectOffset(20, 10, 0, 5)
            };
        }

        private GUIStyle GetErrorStyle()
        {
            return new GUIStyle(EditorStyles.wordWrappedLabel)
            {
                fontSize = 12,
                alignment = TextAnchor.UpperCenter,
                wordWrap = true,
                normal = { textColor = Color.red },
                margin = new RectOffset(10, 10, 5, 5)
            };
        }

        private GUIStyle GetLoadingStyle()
        {
            return new GUIStyle(EditorStyles.wordWrappedLabel)
            {
                fontSize = 12,
                alignment = TextAnchor.MiddleCenter,
                wordWrap = true,
                margin = new RectOffset(10, 10, 5, 5),
                normal = { textColor = Color.blue }
            };
        }
    }

    public class NewsItem
    {
        public string Date { get; set; }
        public string Headline { get; set; }
        public string Body { get; set; }
    }


    public class AssetLink
    {
        public string Title { get; set; }
        public string URL { get; set; }
    }
}
