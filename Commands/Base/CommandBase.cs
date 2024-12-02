using NodeSystem.Commands.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodeSystem.Commands
{
  
    public abstract class CommandBase : ICommand
    {
        public Guid Id { get; } = Guid.NewGuid();

        public abstract string Name { get; }

        public abstract string Description { get; }

        public virtual List<CliArgument>? Arguments { get => null; }
        public virtual List<CliArgument>? OptionalArgs { get => Arguments?.Where(x => x.IsOptional).ToList(); }
        public virtual List<CliArgument>? RequiredArgs { get => Arguments?.Where(x => !x.IsOptional).ToList(); }

        public abstract void Execute(object paramters);

        public void DisplayArgsInfo()
        {
            Console.Write($"Command: ");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(Name.ToUpper().PadRight(43));
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"{Description}");

            if (RequiredArgs != null && RequiredArgs.Count > 0)
            {
                Console.WriteLine("Required Arguments: ");
                foreach (var arg in RequiredArgs)
                {
                    Console.WriteLine("--" + arg.Name.ToUpper().PadRight(20) + $"VALUES:[{string.Join("|", arg.Values ?? new ())}]".PadRight(30) + arg.Description);
                }
                Console.WriteLine("");

            }
            if (OptionalArgs != null && OptionalArgs.Count > 0)
            {
                Console.WriteLine("Optional Arguments: ");
                foreach (var arg in OptionalArgs)
                {
                    Console.WriteLine("--" + arg.Name.ToUpper().PadRight(20) + $"VALUES:[{string.Join("|", arg.Values ?? new())}]".PadRight(30) + arg.Description);
                }
                Console.WriteLine("");
            }
        }

        public string? ValidateArgs(string[] insertedArgs)
        {
            if (Arguments == null)
            {
                return null;
            }

            // Check for missing arguments
            var missingArgs = Arguments
                .Where(arg => !insertedArgs.Contains(arg.Name) && !arg.IsOptional)
                .ToArray();

            if (missingArgs.Length > 0)
            {
                string processedMissingArgs = string.Join(
                    "\n",
                    missingArgs.Select(arg => $"--{arg.Name.ToUpper().PadRight(40)} {arg.Description}")
                );

                return $"Missing arguments: \n{processedMissingArgs}";
            }

            // Check for extra arguments
            var extraArgs = insertedArgs
                .Where(arg => !Arguments.Any(a => a.Name == arg))
                .ToArray();

            if (extraArgs.Length > 0)
            {
                return $"Extra arguments found: {string.Join(", ", extraArgs)}";
            }

            // Check for repetition
            var repeatedArgs = insertedArgs
                .GroupBy(arg => arg)
                .Where(group => group.Count() > 1)
                .Select(group => group.Key)
                .ToArray();

            if (repeatedArgs.Length > 0)
            {
                return $"Repetition detected: {string.Join(", ", repeatedArgs)}";
            }

            // All checks passed
            return null;
        }

        public void ValidateArgs(Dictionary<string, string> insertedArgs)
        {
            if (Arguments == null)
            {
                return;
            }

            // Check for missing arguments
            var missingArgs = Arguments
                .Where(arg => !insertedArgs.ContainsKey(arg.Name) && !arg.IsOptional)
                .ToArray();

            if (missingArgs.Length > 0)
            {
                string processedMissingArgs = string.Join(
                    "\n",
                    missingArgs.Select(arg => $"--{arg.Name.ToUpper().PadRight(40)} {arg.Description}")
                );
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine($"Missing arguments: \n{processedMissingArgs}");
                Console.ForegroundColor = ConsoleColor.White;
            }

            var reqArgNullValue = Arguments
                .Where(arg => insertedArgs.ContainsKey(arg.Name))
                .ToArray();

            var missingArgumentsText = new StringBuilder();

            foreach (var item in reqArgNullValue)
            {
                // Check if the argument exists in the dictionary
                if (insertedArgs.TryGetValue(item.Name, out var value) && string.IsNullOrEmpty(value))
                {
                    // Append the formatted message for each missing argument
                    missingArgumentsText.AppendLine(
                        $"Argument {item.Name.ToUpper()} missing value".PadRight(40) + $"{item.Description}"
                    );
                    missingArgumentsText.AppendLine(
                       $"Accepted values:\n {string.Join("\n ", item.Values)}"
                   );
                }
            }
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine(missingArgumentsText.ToString());
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
