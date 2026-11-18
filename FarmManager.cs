using OpenCvSharp;
using WindowsInput.Native;

namespace MGQP_Script;

public class FarmManager
{
    private InputManager _inputManager;
    private ImageManager _imageManager;
    
    public FarmManager()
    {
        _inputManager = new InputManager();
        _imageManager = new ImageManager();
    }

    public void Coliseum(int level, int runs)
    {
        Thread.Sleep(2000);
        int counter = 0;
        _inputManager.KeyDown(VirtualKeyCode.VK_A);
        while (counter < runs)
        {
            _inputManager.Press(VirtualKeyCode.VK_Z);
            Thread.Sleep(400);
            _inputManager.Press(VirtualKeyCode.VK_Z);
            Thread.Sleep(200);
            switch (level)
            {
                case 1:
                    _inputManager.Press(VirtualKeyCode.VK_Z);
                    break;
                case 2:
                    _inputManager.Press(VirtualKeyCode.DOWN);
                    _inputManager.Press(VirtualKeyCode.VK_Z);
                    break;
                case 3:
                    _inputManager.Press(VirtualKeyCode.DOWN);
                    _inputManager.Press(VirtualKeyCode.DOWN);
                    _inputManager.Press(VirtualKeyCode.VK_Z);
                    break;
                case 4:
                    Console.WriteLine("Bugagaga");
                    _inputManager.Press(VirtualKeyCode.DOWN);
                    _inputManager.Press(VirtualKeyCode.DOWN);
                    _inputManager.Press(VirtualKeyCode.DOWN);
                    _inputManager.Press(VirtualKeyCode.VK_Z);
                    break;
            }
            
            Thread.Sleep(1500);
            while (true)
            {
                if (_imageManager.IsLeftBattlePanel())
                {
                    _inputManager.Press(VirtualKeyCode.VK_Z);
                    _inputManager.Press(VirtualKeyCode.VK_Z);
                }
                else if (_imageManager.isColiseum())
                {
                    Thread.Sleep(1000);
                    counter++;
                    break;
                }
                Thread.Sleep(300);
            }
            
        }
        _inputManager.KeyUp(VirtualKeyCode.VK_A);
    }

    public void FarmEntities(int amount)
    {
        Thread.Sleep(2000);
        Mat  startPicture = _imageManager.GetImage();
        int counter = 0;
        bool isInFight = false;
        _inputManager.KeyDown(VirtualKeyCode.VK_A);
        while (counter < amount)
        {
            if (_imageManager.IsBlackScreen() && !isInFight)
            {
                while (true)
                {
                    if (_imageManager.IsLeftBattlePanel())
                    {
                        _inputManager.Press(VirtualKeyCode.VK_Z);
                        _inputManager.Press(VirtualKeyCode.VK_Z);
                        isInFight = true;
                        break;
                    }
                }
            }
            else if (!_imageManager.isBottomPanel() && isInFight)
            {
                isInFight = false;
                counter++;
            }

            if (!isInFight)
            {
                _inputManager.Press(VirtualKeyCode.RIGHT);
                _inputManager.Press(VirtualKeyCode.LEFT);
            }

        }
        _inputManager.KeyUp(VirtualKeyCode.VK_A);
        Console.WriteLine($"Killed {amount} entities");
    }
}