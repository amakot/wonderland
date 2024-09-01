using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;
using System.Threading;

namespace projectForGays
{
    public class GameNode
    {
        public int Id { get; set; } // ID uzlu
        public string Text { get; set; } // Text uzlu
        public List<Answer> Answers { get; set; } // Seznam odpovědí
        public int Tries { get; set; } // Počet pokusů
    }

    public class Answer
    {
        public string Text { get; set; } // Text odpovědi
        public int NextNodeId { get; set; } // ID následujícího uzlu
    }

    public class Game
    {
        public List<GameNode> Nodes { get; set; } // Seznam uzlů hry
    }
    
    public class GameState
    {
        public int CurrentNodeId { get; set; } // ID aktuálního uzlu
        public int Tries { get; set; } // Počet zbývajících pokusů
        public string PlayerName { get; set; } // Jméno hráče
    }

    class Program
    {
        static string? playerName; // Jméno hráče
        
        static void SaveGame(GameState gameState, string saveFileName)
        {
            string json = JsonConvert.SerializeObject(gameState, Formatting.Indented); // Převede objekt GameState do formátu JSON s formátováním pro čtení
            File.WriteAllText(saveFileName, json); // Zapíše JSON do souboru
        }
        
        static GameState LoadGame(string saveFileName)
        {
            if (File.Exists(saveFileName))
            {
                string json = File.ReadAllText(saveFileName); // Načte obsah souboru do řetězce JSON
                GameState gameState = JsonConvert.DeserializeObject<GameState>(json); // Převede JSON na objekt GameState
                Console.WriteLine("Game loaded successfully."); // Vypíše, že načtení bylo úspěšné  
                return gameState; // Vrátí načtený stav hry
            }
            else
            {
                Console.WriteLine("Save file not found."); // Informuje, že soubor s uložením nebyl nalezen
                return null; // Vrátí null, pokud soubor neexistuje
            }
        }


        static void SlowPrint(string text, int delay)
        {
            Console.WriteLine(); // Zajistíme, aby text začal na novém řádku
            foreach (char c in text)
            {
                // Zkontroluje, zda byla stisknuta klávesa
                if (Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Spacebar)
                {
                    Console.Write(text.Substring(text.IndexOf(c))); // Okamžitě vypíše zbytek textu
                    break;
                }

                // Vypíše znak
                Console.Write(c);

                // Počká určené zpoždění
                Thread.Sleep(delay);
            }

            Console.WriteLine(); // Zajistíme, že končíme novým řádkem
        }
        
        static void PrintColoredLine(string text, ConsoleColor color)
        {
            Console.ForegroundColor = color; // Nastaví barvu textu
            Console.WriteLine(text); // Vypíše text na konzoli
            Console.ResetColor(); // Obnoví výchozí barvu konzole
        }
        
        static void SlowPrintColored(string text, int delay, ConsoleColor color)
        {
            Console.ForegroundColor = color; // Nastaví barvu textu
            SlowPrint(text, delay); // Pomalu vypisuje text s určeným zpožděním
            Console.ResetColor(); // Obnoví výchozí barvu konzole
        }


        static void Main(string[] args)
        {
            ShowMainMenu(); // Zobrazí hlavní menu
        }

        static void ShowMainMenu()
        {
            while (true)
            {
                Console.Clear();
                PrintColoredLine("--------------------------------------", ConsoleColor.White);
                PrintColoredLine("Student in the Wonderland", ConsoleColor.Yellow);
                PrintColoredLine("--------------------------------------", ConsoleColor.White);
                PrintColoredLine("[1] Start New Game", ConsoleColor.Green);
                PrintColoredLine("[2] Load Game", ConsoleColor.Green);
                PrintColoredLine("[3] Exit", ConsoleColor.Green);
                PrintColoredLine("Choose an option: ", ConsoleColor.White);

                string choice = Console.ReadLine();
                switch (choice)
                {
                    case "1":
                        StartNewGame();
                        break;
                    case "2":
                        LoadGame();
                        break;
                    case "3":
                        PrintColoredLine("Exiting the game...", ConsoleColor.White);
                        Environment.Exit(0);
                        break;
                    default:
                        PrintColoredLine("Invalid choice. Try again.", ConsoleColor.Red);
                        Thread.Sleep(2000); // Počká 2 sekundy
                        break;
                }
            }
        }

        static void StartNewGame()
        {
            Console.Clear(); // Vyčistí konzoli
            string jsonFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Scenario1.json"); 
            if (!File.Exists(jsonFilePath)) // Kontrolujeme, zda soubor JSON existuje
            {
                PrintColoredLine("Scenario file not found.", ConsoleColor.Red); // Vypíše chybovou zprávu
                Thread.Sleep(2000); // Počká 2 sekundy
                return; // Ukončí metodu
            }

            string json = File.ReadAllText(jsonFilePath); // Přečtení obsahu JSON souboru do řetězce
            Game game = JsonConvert.DeserializeObject<Game>(json); // Převedení JSON na objekt Game
            if (game?.Nodes == null || game.Nodes.Count == 0) // Kontrola, zda byl objekt Game správně inicializován
            {
                PrintColoredLine("Invalid scenario file.", ConsoleColor.Red); // Vypíše chybovou zprávu
                Thread.Sleep(2000); // Počká 2 sekundy
                return; // Ukončí metodu
            }

            PlayGame(game, 1); // Spustí hru od prvního uzlu
        }

       static void LoadGame() // Metoda pro načtení hry
       {
           GameState gameState = LoadGame(@"C:\Games\WonderLand\WonderLand\SaveGame.json"); // Načte stav hry ze souboru JSON
           if (gameState != null) // Kontrola, zda byl stav hry úspěšně načten
           {
               string jsonFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Scenario1.json"); // Získá cestu k souboru JSON
               if (!File.Exists(jsonFilePath)) // Kontroluje, zda soubor JSON existuje
               {
                   PrintColoredLine("Scenario file not found.", ConsoleColor.Red); // Vypíše chybovou zprávu
                   Thread.Sleep(2000); // Počká 2 sekundy
                   return; // Ukončí metodu
               }
       
               string json = File.ReadAllText(jsonFilePath); // Přečte obsah souboru JSON do řetězce
               Game game = JsonConvert.DeserializeObject<Game>(json); // Převede JSON na objekt Game
               if (game?.Nodes == null || game.Nodes.Count == 0) // Kontrola, zda byl objekt Game správně inicializován
               {
                   PrintColoredLine("Invalid scenario file.", ConsoleColor.Red); // Vypíše chybovou zprávu
                   Thread.Sleep(2000); // Počká 2 sekundy
                   return; // Ukončí metodu
               }
       
               PlayGame(game, gameState.CurrentNodeId); // Spustí hru od aktuálního uzlu
           }
           else
           {
               PrintColoredLine("Failed to load the game.", ConsoleColor.Red); // Vypíše chybovou zprávu
               Thread.Sleep(2000); // Počká 2 sekundy
               ShowMainMenu(); // Zobrazí hlavní menu
           }
       }


        static void PlayGame(Game game, int startNodeId) // Metoda pro spuštění hry
        {
            if (game?.Nodes == null || game.Nodes.Count == 0) // Kontrola, zda byl objekt Game správně inicializován
            {
                PrintColoredLine("Error: Game nodes are not properly initialized.", ConsoleColor.Red); // Vypíše chybovou zprávu
                return; // Ukončí metodu
            }

            int currentNodeId = startNodeId; // ID aktuálního uzlu
            GameState gameState = new GameState { CurrentNodeId = currentNodeId, Tries = 3, PlayerName = playerName }; // Vytvoří nový objekt GameState


            while (true)
            {
                Console.Clear(); // Vyčistí konzoli
                GameNode currentNode = game.Nodes.FirstOrDefault(node => node.Id == currentNodeId); // Získá aktuální uzel hry
                if (currentNode == null) // Kontrola, zda byl uzel nalezen
                {
                    PrintColoredLine("Error: Invalid node ID.", ConsoleColor.Red); // Vypíše chybovou zprávu
                    break; // Ukončí smyčku
                }

                if (currentNode.Id == 11)
                {
                    SlowPrint(currentNode.Text, 50); // Zobrazi text pro uzel 11
                    PrintColoredLine("Enter your name: ", ConsoleColor.White); // Vyzve hráče, aby zadal sve jméno
                    playerName = Console.ReadLine(); // Přečte jméno hráče
                    gameState.PlayerName = playerName; // Aktualizace gameState
                    currentNodeId = 12; // Automaticky přejí na uzel 12
                    gameState.CurrentNodeId = currentNodeId; // Aktualizace gameState
                    continue;
                }
                
                if (currentNode.Id == 13)
                {
                    while (currentNode.Tries > 0) // Pokud je počet pokusů větší než 0
                    {
                        Console.Clear(); // Vyčistí konzoli
                        SlowPrint(currentNode.Text, 50); // Pomalu vypíše text pro uzel 13

                        for (int i = 0; i < currentNode.Answers.Count; i++) // Prochází všechny odpovědi
                        {
                            Console.WriteLine($"{i + 1}. {currentNode.Answers[i].Text}"); // Vypíše odpovědi 
                        }

                        PrintColoredLine("Choose an option: ", ConsoleColor.White); // Vyzve hráče, aby zadal volbu
                        if (int.TryParse(Console.ReadLine(), out int chosenOption13) && chosenOption13 > 0 && chosenOption13 <= currentNode.Answers.Count)
                        {
                            int nextNodeId = currentNode.Answers[chosenOption13 - 1].NextNodeId; // Získá ID následujícího uzlu
                            if (nextNodeId == 14)
                            {
                                SlowPrint("The Caterpillar looks at you and says, 'We have someone that doesn't read pages, aren't we? New technologies made ways for a people to shut their brains and be led by quick information. Anyway, you're wrong'", 50); // Text pro uzel 14
                                currentNode.Tries--; // Sníží počet pokusů na 1
                                gameState.Tries = currentNode.Tries; // Aktualizace gameState
                                SlowPrint($"You have {currentNode.Tries} tries left.", 50); // Vypíše, kolik zbývá pokusů
                                SlowPrint("Try again? Press Enter to continue.", 50); // Vyzve hráče, aby zadal Enter
                                Console.ReadLine();  // Přečte vstup od hráče

                                if (currentNode.Tries == 0) // Pokud je počet pokusů roven 0
                                {
                                    PrintColoredLine("You have exhausted all your tries. Game over.", ConsoleColor.Red); // Vypíše, že hráč vyčerpal všechny pokusy
                                    break; // Ukončí cyklu
                                }
                            }
                            else
                            {
                                currentNodeId = nextNodeId; // Nastaví ID následujícího uzlu
                                gameState.CurrentNodeId = currentNodeId; // Aktualizace gameState
                                break; // Ukončí cyklu
                            }
                        }
                        else
                        {
                            PrintColoredLine("Invalid choice, try again.", ConsoleColor.Red); // Vypíše, že volba je neplatná
                        }
                    }
                    continue; // Pokračuje v cyklu
                }

                if (currentNode.Id == 16)
                {
                    while (currentNode.Tries > 0) // Pokud je počet pokusů větší než 0
                    {
                        Console.Clear(); // Vyčistí konzoli
                        SlowPrint(currentNode.Text, 50); // Pomalu vypíše text pro uzel 16

                        for (int i = 0; i < currentNode.Answers.Count; i++) // Prochází všechny odpovědi
                        {
                            Console.WriteLine($"{i + 1}. {currentNode.Answers[i].Text}"); // Vypíše odpovědi
                        }

                        PrintColoredLine("Choose an option: ", ConsoleColor.White); // Vyzve hráče, aby zadal volbu
                        if (int.TryParse(Console.ReadLine(), out int chosenOption16) && chosenOption16 > 0 && chosenOption16 <= currentNode.Answers.Count) // Kontroluje, zda byla zadaná platná volba
                        {
                            int nextNodeId = currentNode.Answers[chosenOption16 - 1].NextNodeId; // Získá ID následujícího uzlu
                            if (nextNodeId == 17)
                            {
                                SlowPrint("The Caterpillar looks at you and says, 'Maybe you are not so smart as I thought. You know that life can be hard sometimes, and only remembering something can't give you safety. To be agile in this life you need to use your logics.'", 50); // Text pro uzel 17
                                currentNode.Tries--; // Sníží počet pokusů na 1
                                gameState.Tries = currentNode.Tries; // Aktualizace gameState
                                SlowPrint($"You have {currentNode.Tries} tries left.", 50); // Vypíše, kolik zbývá pokusů
                                Console.WriteLine("Try again? Press 1 or Enter to continue."); // Vyzve hráče, aby zadal Enter
                                string retryInput = Console.ReadLine(); // Přečte vstup od hráče
                                if (currentNode.Tries == 0)
                                {
                                    PrintColoredLine("You have exhausted all your tries. Game over.", ConsoleColor.Red); // Vypíše, že hráč vyčerpal všechny pokusy
                                    break; // Ukončí cyklu
                                }
                            }
                            else
                            {
                                currentNodeId = nextNodeId; // Nastaví ID následujícího uzlu
                                gameState.CurrentNodeId = currentNodeId; // Aktualizace gameState
                                break; // Ukončí cyklu
                            }
                        }
                        else
                        {
                            PrintColoredLine("Invalid choice, try again.", ConsoleColor.Red); // Vypíše, že volba je neplatná
                        }
                    }
                    continue; // Pokračuje v cyklu
                }
                
                if (currentNode.Id == 18)
                {
                    Console.Clear(); // Vyčistí konzoli
                    SlowPrint(currentNode.Text, 50); // Pomalu vypíše text pro uzel 18
                    PrintColoredLine("Enter your answer: ", ConsoleColor.White); // Vyzve hráče, aby zadal odpověď
                    string userAnswer = Console.ReadLine().ToLower(); // Přečte odpověď od hráče a převede na malá písmena

                    if (userAnswer == "mario") // Kontroluje, zda odpověď uživatele byla správná
                    {
                        currentNodeId = 19;
                        gameState.CurrentNodeId = currentNodeId; // Aktualizace gameState
                    }
                    else
                    {
                        SlowPrint("Wrong answer. Try again.", 50); // Vypíše chybovou zprávu
                        SlowPrint($"You have {currentNode.Tries} tries left.", 50); // Vypíše, kolik zbývá pokusů
                        currentNode.Tries--; // Sníží počet pokusů na 1
                        gameState.Tries = currentNode.Tries; // Aktualizuje počet pokusů ve stavu hry (gameState)
                        if (currentNode.Tries == 0)
                        {
                            PrintColoredLine("You have exhausted all your tries. Game over.", ConsoleColor.Red); // Vypíše, že hráč vyčerpal všechny pokusy
                            break; // Ukončí cyklu
                        }
                    }

                    continue; // Pokračuje v cyklu
                }

                SlowPrint(currentNode.Text.Replace("{playerName}", playerName), 50); //// Vypíše text uzlu s nahrazením jménem hráče

                Console.WriteLine(); // Přidá nový řádek před zobrazením možností odpovědi

                for (int i = 0; i < currentNode.Answers.Count; i++) // Prochází všechny odpovědi
                {
                    PrintColoredLine($"{i + 1}. {currentNode.Answers[i].Text}", ConsoleColor.White); // Vypíše odpovědi
                }
                
                
                for (int i = 0; i < 4; i++) // Přidá prázdné řádky před zobrazením možností pro uložení a ukončení
                {
                    Console.WriteLine();
                }
                PrintColoredLine("[S] Save Game", ConsoleColor.White); // Vypíše možnost uložení hry
                PrintColoredLine("[Q] Quit Game", ConsoleColor.White); // Vypíše možnost ukončení hry
                PrintColoredLine("Choose an option: ", ConsoleColor.White); // Vyzve hráče, aby zadal volbu

                
                    
                    string input = Console.ReadLine(); // Přečte vstup od hráče
                    
                    if (input.ToLower() == "s") // Kontroluje, zda hráč zadal "s" pro uložení hry
                    {
                        SaveGame(gameState, @"C:\Games\WonderLand\WonderLand\SaveGame.json"); // Uloží hru do JSON souboru
                        PrintColoredLine("Game saved successfully.", ConsoleColor.White); // Vypíše, že hra byla úspěšně uložena
                        Thread.Sleep(2000); // Počká 2 sekundy
                    }
                    else if (input.ToLower() == "q") // Kontroluje, zda hráč zadal "q" pro ukončení hry
                    {
                        PrintColoredLine("Exiting to main menu...", ConsoleColor.White); // Vypíše, že se hra ukončuje
                        Thread.Sleep(2000); // Počká 2 sekundy
                        ShowMainMenu(); // Zobrazí hlavní menu
                        break; // Ukončí cyklus
                    }
                    else if (int.TryParse(input, out int chosenOption) && chosenOption > 0 && chosenOption <= currentNode.Answers.Count) // Kontroluje, zda byla zadaná platná volba
                    {
                        int nextNodeId = currentNode.Answers[chosenOption - 1].NextNodeId; // Získá ID následujícího uzlu
                        if (nextNodeId == 0) // Kontroluje, zda je ID následujícího uzlu rovno 0
                        {
                            ShowMainMenu(); // Zobrazí hlavní menu
                            break; // Ukončí cyklu
                        }

                    if (nextNodeId == -1)
                    {
                        PrintColoredLine("Wrong answer. Try again.", ConsoleColor.Red); // Vypíše chybovou zprávu
                        currentNode.Tries--; // Sníží počet pokusů na 1
                        gameState.Tries = currentNode.Tries; // Aktualizace gameState
                        if (currentNode.Tries == 0)
                        {
                            PrintColoredLine("You have exhausted all your tries. Game over.", ConsoleColor.Red); // Vypíše, že hráč vyčerpal všechny pokusy
                            break; // Ukončí cyklu
                        }
                    }
                    else
                    {
                        currentNodeId = nextNodeId; // Nastaví ID následujícího uzlu
                        gameState.CurrentNodeId = currentNodeId; // Aktualizace gameState
                    }
                }
                else
                {
                    PrintColoredLine("Invalid choice, try again.", ConsoleColor.Red); // Vypíše, že volba je neplatná
                }
            }

            PrintColoredLine("Press any key to return to the main menu...", ConsoleColor.White); // Vyzve hráče, aby stiskl libovolnou klávesu pro návrat do hlavního menu
            Console.ReadKey(); // Čeká na stisk klávesy od hráče
            ShowMainMenu(); // Zobrazí hlavní menu
        }
    }
}
