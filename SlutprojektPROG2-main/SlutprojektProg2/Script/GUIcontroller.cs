public class GUIcontroller
{
    public void Draw(int hp)
    {
        Raylib.DrawRectangle(Game.ScreenWidth - 255, 55, 245, 35, Color.Gray); //bakgrund
        Raylib.DrawRectangle(Game.ScreenWidth - 250, 60, (int)(2.35f * hp), 25, Color.Red); //HP
        Raylib.DrawText($"{hp}", Game.ScreenWidth - 50, 90, 30, Color.White);
    }
}