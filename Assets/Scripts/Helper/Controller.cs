using XInputDotNetPure;

public static class Controller
{
    public static bool LeftPress(GamePadState prevState, GamePadState state, float threshold = 0.3f)
    {
        return state.DPad.Left == ButtonState.Pressed && prevState.DPad.Left == ButtonState.Released ||
               state.ThumbSticks.Left.X < -threshold && prevState.ThumbSticks.Left.X > -threshold ||
               state.Buttons.LeftShoulder == ButtonState.Pressed && prevState.Buttons.LeftShoulder == ButtonState.Released;
    }

    public static bool RightPress(GamePadState prevState, GamePadState state, float threshold = 0.3f)
    {
        return state.DPad.Right == ButtonState.Pressed && prevState.DPad.Right == ButtonState.Released ||
               state.ThumbSticks.Left.X > threshold && prevState.ThumbSticks.Left.X < threshold ||
               state.Buttons.RightShoulder == ButtonState.Pressed && prevState.Buttons.RightShoulder == ButtonState.Released;
    }

    public static void EndAllVibration()
    {
        for (PlayerIndex i = 0; i <= PlayerIndex.Four; i++)
            GamePad.SetVibration(i, 0, 0);
    }
}
