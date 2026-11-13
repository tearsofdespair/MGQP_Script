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
        int counter = 0;
        while (counter < runs)
        {
            Mat startPicture = _imageManager.GetImage();
            _inputManager.Press(VirtualKeyCode.VK_Z);
            _inputManager.Hold(VirtualKeyCode.VK_A, 600);
            _inputManager.Press(VirtualKeyCode.VK_Z);
            Thread.Sleep(200);
            switch (level)
            {
                case 0:
                    _inputManager.Press(VirtualKeyCode.VK_Z);
                    break;
                case 1:
                    _inputManager.Press(VirtualKeyCode.DOWN);
                    _inputManager.Press(VirtualKeyCode.VK_Z);
                    break;
                case 2:
                    _inputManager.Press(VirtualKeyCode.DOWN);
                    _inputManager.Press(VirtualKeyCode.DOWN);
                    _inputManager.Press(VirtualKeyCode.VK_Z);
                    break;
                case 3:
                    Console.WriteLine("Bugagaga");
                    _inputManager.Press(VirtualKeyCode.DOWN);
                    _inputManager.Press(VirtualKeyCode.DOWN);
                    _inputManager.Press(VirtualKeyCode.DOWN);
                    _inputManager.Press(VirtualKeyCode.VK_Z);
                    break;
            }
            _inputManager.KeyDown(VirtualKeyCode.VK_A);
            
            bool trigger = false;
            while (!trigger)
            {
                while (true)
                {
                    if (_imageManager.AreImagesSimilar(startPicture, _imageManager.GetImage()))
                    {
                        trigger = true;
                        break;
                    }
                    else if (_imageManager.IsLeftBattlePanel())
                    {
                        break;
                    }
                    Thread.Sleep(300);
                }

                if (!trigger)
                {
                    _inputManager.Press(VirtualKeyCode.VK_Z);
                    _inputManager.Press(VirtualKeyCode.VK_Z);
                }
            }
            _inputManager.KeyUp(VirtualKeyCode.VK_A);
            counter++;
        }
        Console.WriteLine($"Finished: {runs} runs");
    }

    public void FarmEntities(int amount)
    {
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