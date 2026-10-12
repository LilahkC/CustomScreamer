using System.Numerics;
using System.Runtime.InteropServices;
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
            if (ImGui.Button("Play"))
            {
                SDL.LogInfo(SDL.LogCategory.Application, "User pressed 'Play' in Settings Window");
                Settings.Save();
                Window.StartGame = true;
            }
            
            if (ImGui.Button("Test"))
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
                    ShowConfirmation($"{dialogResult.Path} successfully selected");
                }
            }
            
            if (ImGui.GetTime() < ConfirmationHideTime)
            {
                ImGui.PushStyleColor(ImGuiCol.Text, (Vector4)new(0.133f, 0.941f, 0.239f, 1f));
                ImGui.TextWrapped(confirmationText);
                ImGui.PopStyleColor();
            }
            
            ImGui.NewLine();
            
            if (ImGui.Button("Save Config"))
                Settings.Save();
            
            if (ImGui.Button("Load Config"))
                Settings.Load();

            if (ImGui.Button("Delete Save"))
                Settings.DeleteSave();

            ImGui.NewLine();
            
            //if (ImGui.Button("Connect"));

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
}
