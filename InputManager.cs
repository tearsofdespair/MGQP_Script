using WindowsInput;
using WindowsInput.Native;

namespace MGQP_Script;

public class InputManager
{
    private InputSimulator _sim;
    
    public InputManager()
    {
        _sim = new InputSimulator();
    }
    
    public void Press(VirtualKeyCode keyCode)
    {
        TimeBreak();
        _sim.Keyboard.KeyDown(keyCode);
        Thread.Sleep(30);
        _sim.Keyboard.KeyUp(keyCode);
        TimeBreak();
    }

    public void Hold(VirtualKeyCode keyCode, int miliseconds)
    {
        TimeBreak();
        _sim.Keyboard.KeyDown(keyCode);
        Thread.Sleep(miliseconds);
        _sim.Keyboard.KeyUp(keyCode);
        TimeBreak();
    }

    public void KeyDown(VirtualKeyCode keyCode)
    {
        _sim.Keyboard.KeyDown(keyCode);
    }
    
    public void KeyUp(VirtualKeyCode keyCode)
    {
        _sim.Keyboard.KeyUp(keyCode);
    }

    private void TimeBreak()
    {
        Thread.Sleep(20);
    }
}