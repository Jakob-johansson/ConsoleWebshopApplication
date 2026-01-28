using System;
using System.Collections.Generic;
using System.Text;
using WebshopConsole.Models;
using WebshopConsole.Services;
namespace WebshopConsole.Services
{
    internal class DrawService
    {
        public static void DrawBox(int left, int top, int width, int height, string title = "")
        {
            // Top
            Console.SetCursorPosition(left, top);
            Console.Write("┌" + new string('─', width - 2) + "┐");

            // Title
            if (!string.IsNullOrEmpty(title))
            {
                Console.SetCursorPosition(left + 2, top);
                Console.Write(title);
            }

            // Sides
            for (int i = 1; i < height - 1; i++)
            {
                Console.SetCursorPosition(left, top + i);
                Console.Write("│");
                Console.SetCursorPosition(left + width - 1, top + i);
                Console.Write("│");
            }

            // Bottom
            Console.SetCursorPosition(left, top + height - 1);
            Console.Write("└" + new string('─', width - 2) + "┘");
        }


    }
}
