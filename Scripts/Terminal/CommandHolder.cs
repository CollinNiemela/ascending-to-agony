/*
* CommandHolder class used to hold key function pairs
* Uses an underlying dictionary and list to hold values
* 
* Note: this class is not case sensitive
*/

using System;
using System.Collections.Generic;
using System.Reflection;

public partial class CommandHolder
{
    Dictionary<string, KeyValuePair<KeyValuePair<string, string>, Delegate>> commands;
    List<string> commandList;

    //public constructor
    public CommandHolder()
    {
        commands = new Dictionary<string, KeyValuePair<KeyValuePair<string, string>, Delegate>>();
        commandList = new List<string>();
    }

    //inserts the function into the dictionary
    public void insert(string command, string help, Delegate function)
    {
        if (command.Length > 16) return; //commands need to be less than 16 characters
        commands.Add(command.ToUpper(), new KeyValuePair<KeyValuePair<string, string>, Delegate>
                    (new KeyValuePair<string, string>(help, ""), function));
        commandList.Insert(0, command.ToUpper());
        commandList.Sort();
    }

    //removes the function from the dictionary
    public void remove(string command)
    {
        if (commands.ContainsKey(command.ToUpper()))
        {
            commands.Remove(command.ToUpper());
            commandList.Remove(command.ToUpper());
        }
    }

    //returns true if the dictionary contains the command
    public bool contains(string command)
    {
        return commands.ContainsKey(command.ToUpper());
    }

    //clears the command dictionary and list
    public void clear()
    {
        commands.Clear();
        commandList.Clear();
    }

    //runs the given command found in the dictionary
    //return function return type
    //returns null if invalid command is used
    public KeyValuePair<string[], object> run(string command)
    {
        command = command.ToUpper();
        List<string> output = new List<string>();
        string[] commandSplit = command.Split(' ');

        //lists the commands available if help is entered
        if (commandSplit.Length == 1 && commandSplit[0].ToUpper() == "HELP")
        {
            output.Add("List of commands available:");
            string list = "";
            foreach (string c in commandList)
            {
                list += c.PadRight(20).ToUpper() + commands[c].Key.Key + "\n";
            }
            if (list != "")
            {
                output.Add(list.Substring(0, list.Length - 1));
                output.Add("Type \"COMMAND_NAME help\" for command information");
            }
            else
            {
                output.Add("No commands available.");
            }
            return new KeyValuePair<string[], object>(output.ToArray(), null);
        }

        //gives an error is the command is not found
        if (commandSplit.Length <= 0 || !commands.ContainsKey(commandSplit[0]))
        {
            output.Add("Error: Invalid command");
            return new KeyValuePair<string[], object>(output.ToArray(), null);
        }

        //retrieves the function delegate
        Delegate function = commands[commandSplit[0]].Value;

        if (function == null)
        {
            if (commandSplit.Length == 2 && commandSplit[1] == "help")
            {
                output.Add(commands[commandSplit[0]].Key.Key);
                output.Add(commandSplit[0]);
            }
            else
            {
                output.Add("Error: Incorrect number of parameters used");
            }
            return new KeyValuePair<string[], object>(output.ToArray(), null);
        }

        //retrieves the parameter information
        ParameterInfo[] parameters = function.Method.GetParameters();

        //prints out help information if asked in command
        if (commandSplit.Length > 1 && commandSplit[1].ToUpper() == "HELP")
        {
            //checks if the command is already cached by looking if the value is not blank
            if (commands[commandSplit[0]].Key.Value != "")
            {
                output.Add(commands[commandSplit[0]].Key.Value);
            }
            else
            {
                string parameterString = "";
                for (int i = 0; i < parameters.Length; i++)
                {
                    parameterString += " " + parameters[i].ParameterType.ToString().ToLower() + ":" + parameters[i].Name;
                }
                parameterString = parameterString.Replace("system.", "").Replace("32", "").Replace("single", "float");
                //combines everything together then adds the new string to the command dictionary
                string help = commands[commandSplit[0]].Key.Key + "\n" + commandSplit[0] + parameterString;
                output.Add(help);
                commands[commandSplit[0]] = new KeyValuePair<KeyValuePair<string, string>, Delegate>
                    (new KeyValuePair<string, string>(commands[commandSplit[0]].Key.Key, help),
                    commands[commandSplit[0]].Value);
            }
            return new KeyValuePair<string[], object>(output.ToArray(), null);
        }

        //sets the string to the correct types and runs the function
        if (commandSplit.Length - 1 != parameters.Length)
        {
            output.Add("Error: Incorrect number of parameters used");
            return new KeyValuePair<string[], object>(output.ToArray(), null);
        }
        object[] convertedParameters = new object[parameters.Length];
        for (int i = 0; i < parameters.Length; i++)
        {
            try
            {
                convertedParameters[i] = Convert.ChangeType(commandSplit[i + 1], parameters[i].ParameterType);
            }
            catch
            {
                output.Add("Error: Incorrect parameter type used");
                return new KeyValuePair<string[], object>(output.ToArray(), null);
            }
        }
        try
        {
            return new KeyValuePair<string[], object>(output.ToArray(), function.DynamicInvoke(convertedParameters));
        }
        catch
        {
            output.Add("Error: Invalid parameters");
            return new KeyValuePair<string[], object>(output.ToArray(), null);
        }
    }

}

