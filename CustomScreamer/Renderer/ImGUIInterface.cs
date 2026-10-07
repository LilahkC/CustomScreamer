using CustomScreamer.Utils;
using SDL3ImGui;
namespace CustomScreamer.Renderer;
using ImGuiNET;
using SDL3;

public class ImGUIInterface
{
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

            ImGui.Checkbox("Open Settings On Launch", ref Game.Game.OpenSettingsOnLaunch);
                
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
}
