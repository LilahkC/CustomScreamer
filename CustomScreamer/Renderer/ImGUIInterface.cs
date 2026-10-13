using System.Numerics;
using CustomScreamer.MediaPlayer;
using CustomScreamer.Utils;
using NativeFileDialogSharp;
using SDL3ImGui;

namespace CustomScreamer.Renderer;
using ImGuiNET;
using SDL3;

public class ImGUIInterface
{
    private static double ConfirmationHideTime => ImGui.GetTime() + 2.0;
    private static string confirmationText = "";
    private static string twitchConfirmationText = "";
    private static Vector4 twitchConfirmationColor = new(1.0f, 1.0f, 1.0f, 1.0f);
    
    public static Vector4 Green => new(0.133f, 0.941f, 0.239f, 1f);
    public static Vector4 Orange => new(0.941f, 0.549f, 0.133f, 1f);
    public static Vector4 Red => new(0.941f, 0.133f, 0.239f, 1f);

    public static void BuildImGUIInterface(ImGuiSDL3 platform, ImGuiSDL3Renderer imGuiRenderer)
    {
        platform.NewFrame();
        imGuiRenderer.NewFrame();
        ImGui.NewFrame();
        
        ImGuiViewportPtr viewport = ImGui.GetMainViewport();
        ImGui.SetNextWindowPos(viewport.WorkPos);
        ImGui.SetNextWindowSize(viewport.WorkSize);

        const ImGuiWindowFlags Flags = ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.NoMove;
            
        if (ImGui.Begin("ImGUI", Flags))
        {
            /* PLAY/TEST */
            
            if (ImGui.Button("Play"))
            {
                SDL.LogInfo(SDL.LogCategory.Application, "User pressed 'Play' in Settings Window");
                Settings.Save();
                Window.StartGame = true;
            }
            
            if (ImGui.Button("Test Screamer"))
            {
                Window.CreateGameWindow(true);
                Game.Game.PlayScreamer(true);
            }
            
            /* SCREAMER PARAMETERS */
            
            ImGui.NewLine();
            
            const ImGuiInputTextFlags NumberOnly = ImGuiInputTextFlags.CharsDecimal;
                
            ImGui.AlignTextToFramePadding();
            ImGui.Text("Chance");
            ImGui.SameLine(55f);
            ImGui.SetNextItemWidth(110f);
            if (ImGui.InputFloat("##Chance", ref Game.Game.Chance, 0.25f, 15f, "%.2f %%", NumberOnly))
                Game.Game.Chance = Math.Clamp(Game.Game.Chance, 0f, 100f);

            ImGui.AlignTextToFramePadding();
            ImGui.Text("Time");
            ImGui.SameLine(40f);
            ImGui.SetNextItemWidth(135f);
            if (ImGui.InputFloat("##Time", ref Game.Game.Time, 0.1f, 1f, "%.1f seconds", NumberOnly))
                Game.Game.Time = MathF.Max(Game.Game.Time, 0.01f);
            
            ImGui.AlignTextToFramePadding();
            ImGui.Text("Volume");
            ImGui.SameLine(60f);
            ImGui.SetNextItemWidth(90f);
            if (ImGui.SliderFloat("##Volume", ref SoundPlayer.Volume, 0.1f, 100f, "%.0f%"))
                SoundPlayer.UpdateVolume(SoundPlayer.Volume / 100);
            
            /* APP PARAMETERS */
            
            ImGui.NewLine();

            ImGui.Checkbox("Open Settings On Launch", ref Game.Game.OpenSettingsOnLaunch);
            
            bool launchAtBoot = LaunchOnBoot.IsOpenOnBootEnabled();
            if (ImGui.Checkbox("Launch at boot", ref launchAtBoot))
                LaunchOnBoot.SetStartup(launchAtBoot);

            if (ImGui.Button("Choose Screamer folder"))
            {
                string userFolder = Folder.GetUserFolder();
                DialogResult dialogResult = Dialog.FolderPicker(string.IsNullOrEmpty(userFolder) ? AppDomain.CurrentDomain.BaseDirectory : userFolder);

                if (dialogResult.IsOk)
                {
                    Folder.SetUserFolder(dialogResult.Path);
                    ShowConfirmation($"{dialogResult.Path} successfully selected");
                }
            }
            
            if (ImGui.GetTime() < ConfirmationHideTime)
            {
                ImGui.PushStyleColor(ImGuiCol.Text, Green);
                ImGui.TextWrapped(confirmationText);
                ImGui.PopStyleColor();
            }
            
            /* TWITCH */

            ImGui.NewLine();

            if (ImGui.Button("Connect"))
            {
                if(string.IsNullOrEmpty(Twitch.ChannelName) || string.IsNullOrEmpty(Twitch.TwitchOAuth))
                    ShowTwitchConfirmation("Please enter your Twitch Channel Name and OAuth", Red);
                else
                    Twitch.Initialize();
            }
            
            if (ImGui.GetTime() < ConfirmationHideTime)
            {
                ImGui.PushStyleColor(ImGuiCol.Text, twitchConfirmationColor);
                ImGui.TextWrapped(twitchConfirmationText);
                ImGui.PopStyleColor();
            }
            
            /* QUIT */
            
            ImGui.NewLine();

            if (ImGui.Button("Quit"))
            {
                Window.Loop = false;
                SDL.LogInfo(SDL.LogCategory.Application, "User pressed 'Quit' in Settings Window");
            }
        }
            
        ImGui.End();
        ImGui.EndFrame();
    }
    
    public static void OnPathDropped(string path)
    {
        if (!Directory.Exists(path))
            return;

        Folder.SetUserFolder(path);
        ShowConfirmation($"{path} successfully selected");
    }

    private static void ShowConfirmation(string text)
    {
        confirmationText = new DirectoryInfo(text).Name;
    }
    
    public static void ShowTwitchConfirmation(string text, Vector4 color)
    {
        twitchConfirmationColor = color;
        twitchConfirmationText = text;
    }
}
