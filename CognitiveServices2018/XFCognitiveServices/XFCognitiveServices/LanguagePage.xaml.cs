using System;
using System.Collections.Generic;
using CognitiveServices.Core;
using Xamarin.Forms;

namespace XFCognitiveServices
{
    public partial class LanguagePage : ContentPage
    {
        public LanguagePage()
        {
            InitializeComponent();
        }

        private async void TranslateButton_Clicked(object sender, EventArgs e)
        {
            var translatedText = string.Empty;
            var translationClient = new TranslatorTextService();

            //半角英数記号のみであれば、日本語に翻訳し、それ以外は英語に翻訳する。
            if (IsHalfSizeAlphamericCharacters(SourceText.Text))
                translatedText = await translationClient.TranslateTextAsync(SourceText.Text, TranslatorTextService.ToLanguage.ja);
            else
                translatedText = await translationClient.TranslateTextAsync(SourceText.Text, TranslatorTextService.ToLanguage.en);

            TranslatedText.Text = translatedText;
        }

        private async void AnalyzeTextButton_Clicked(object sender, EventArgs e)
        {
            var englishText = string.Empty;
            TranslatedText.Text = string.Empty;

            var textAnalysisClient = new TextAnalyticsService();
            var translationClient = new TranslatorTextService();

            // 必要に応じて英語に翻訳してTextAnalyticsに投げる。
            if (IsHalfSizeAlphamericCharacters(SourceText.Text))
                englishText = SourceText.Text;
            else
                englishText = await translationClient.TranslateTextAsync(SourceText.Text, TranslatorTextService.ToLanguage.en);

            // 英語のテキストをText Analyticsに投げて結果を取得。
            var sentiment = await textAnalysisClient.AnalyzeSentimentAsync(englishText);

            // 英語テキストを表示して、テキスト解析結果をアラートで表示。
            TranslatedText.Text = englishText;
            await DisplayAlert("Text Analytics", $"Sentiment(Happiness):\n{sentiment*100:00}%", "OK");
        }

        private void Editor_TextChanged(object sender, TextChangedEventArgs e)
        {
            TranslatedText.Text = string.Empty;

            if (SourceText.Text.Length == 0)
            {
                TranslateButton.IsEnabled = false;
                AnalyzeTextButton.IsEnabled = false;
            }
            else if (SourceText.Text.Length > 0)
            {
                TranslateButton.IsEnabled = true;
                AnalyzeTextButton.IsEnabled = true;
            }
        }

        /// <summary>
        /// 簡易半角英数記号のチェック
        /// </summary>
        /// <returns><c>true</c> , 半角英数記号のみ含まれる, <c>false</c> それ以外（ここでは日本語と解釈）</returns>
        /// <param name="input">Input.</param>
        private bool IsHalfSizeAlphamericCharacters(string input)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(input, @"^[a-zA-Z0-9 -/:-@\[-\`\{-\~]+$");
        }
    }
}
