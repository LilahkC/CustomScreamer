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
    public enum Section { Application, Twitch }
    
    private struct Confirmation
    {
        public string Text;
        public Vector4 Color;
        public double HideTime;
    }

    private static readonly Dictionary<Section, Confirmation> confirmations = new();
    
    public static void BuildImGUIInterface(ImGuiSDL3 platform, ImGuiSDL3Renderer imGuiRenderer)
    {
        platform.NewFrame();
        imGuiRenderer.NewFrame();
        ImGui.NewFrame();
        
        ImGuiViewportPtr viewport = ImGui.GetMainViewport();
        ImGui.SetNextWindowPos(viewport.WorkPos);
        ImGui.SetNextWindowSize(viewport.WorkSize);

        const ImGuiWindowFlags Flags = ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.NoMove;
        const ImGuiTreeNodeFlags CategoryFlags = ImGuiTreeNodeFlags.DefaultOpen;
            
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
                    ShowConfirmation(Section.Application, $"{new DirectoryInfo(dialogResult.Path).Name} successfully selected", Color.Green);
                    
                    if(Twitch.IsConnected())
                        ShowConfirmation(Section.Twitch, $"The chat will be able to use !{Folder.GetSubDirectoriesText()} to play a screamer", Color.Green, 10);
                }

                DrawConfirmation(Section.Application);
            }
            
            ImGui.NewLine();

            /* TWITCH */

            ImGui.SetNextItemWidth(100f);
            string channelName = Twitch.ChannelName ?? "";
            if (ImGui.InputTextWithHint("##TwitchChannel", "Channel Name", ref channelName, 24, ImGuiInputTextFlags.EnterReturnsTrue))
                Twitch.ChannelName = channelName;
            
            bool connected = Twitch.IsConnected();
            
            ImGui.BeginDisabled(connected);
            if (ImGui.Button("Connect to Twitch"))
            {
                if (string.IsNullOrEmpty(Twitch.ChannelName))
                    ShowConfirmation(Section.Twitch, "Please enter your Twitch Channel Name", Color.Red);
                else
                    Twitch.Initialize();
            }

            ImGui.EndDisabled();

            ImGui.BeginDisabled(!connected);

            if (ImGui.Button("Disconnect from Twitch"))
            {
                Twitch.Disconnect();
                ShowConfirmation(Section.Twitch,"Twitch Disconnected", Color.Red);
            }

            ImGui.EndDisabled();

            DrawConfirmation(Section.Twitch);

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
        ShowConfirmation(Section.Application,$"{new DirectoryInfo(path).Name} successfully selected", Color.Green);

        if(Twitch.IsConnected())
            ShowConfirmation(Section.Twitch, $"The chat will be able to use !{Folder.GetSubDirectoriesText()} to play a screamer", Color.Green, 10);
    }

    public static void ShowConfirmation(Section section, string text, Vector4 color, double duration = 2)
    {
        confirmations[section] = new()
        {
            Text = text,
            Color = color,
            HideTime = ImGui.GetTime() + duration
        };
    }

    private static void DrawConfirmation(Section section)
    {
        if (!confirmations.TryGetValue(section, out Confirmation c) || ImGui.GetTime() >= c.HideTime)
            return;

        ImGui.PushStyleColor(ImGuiCol.Text, c.Color);
        ImGui.TextWrapped(c.Text);
        ImGui.PopStyleColor();
    }
}
