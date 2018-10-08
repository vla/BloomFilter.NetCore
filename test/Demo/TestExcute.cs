// ==++==
//
//  Copyright (c) . All rights reserved.
//
// ==--==
/* ---------------------------------------------------------------------------
 *
 * Author			: v.la
 * Email			: v.la@live.cn
 * Created			: 2015-08-27
 * Class			: TestExcute.cs
 *
 * ---------------------------------------------------------------------------
 * */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

/// <summary>
/// Class TestAttribute.
/// </summary>
public class TestAttribute : Attribute
{
    /// <summary>
    /// Gets or sets the description.
    /// </summary>
    /// <value>The description.</value>
    public string Description { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="TestAttribute"/> class.
    /// </summary>
    public TestAttribute()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TestAttribute"/> class.
    /// </summary>
    /// <param name="description">The description.</param>
    public TestAttribute(string description)
    {
        Description = description;
    }
}

/// <summary>
/// Class TestExcute.
/// </summary>
public static class TestExcute
{
    /// <summary>
    /// Excutes the specified test assembly.
    /// </summary>
    /// <param name="t">The t.</param>
    public static void Excute(Type t)
    {
#if NET45
        var dataAccess = Assembly.GetAssembly(t);
#else
        var dataAccess = t.GetTypeInfo().Assembly;
#endif

        IList<ExecuteFunc> list = new List<ExecuteFunc>();

        foreach (var type in dataAccess.GetTypes())
        {
            var clazz = type.GetConstructor(Type.EmptyTypes);
            if (clazz == null) continue;
            foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.Instance))
            {
                var attr = method.GetCustomAttributes(typeof(TestAttribute), false).FirstOrDefault() as TestAttribute;
                if (attr != null)
                {
                    object instance = Activator.CreateInstance(type);
                    ExecuteFunc func = new ExecuteFunc(instance, method, attr.Description);
                    list.Add(func);
                }
            }
        }

        if (list.Count > 0)
        {
            StringBuilder text = new StringBuilder();

            lrTag("Select the use-case", "-", 20);

            for (int i = 0; i < list.Count; i++)
            {
                text.AppendFormat("[{0}] {1}{2}", i + 1, list[i], Environment.NewLine);
            }

            text.AppendLine("\r\n[0] \texit. ");
            string _display = text.ToString();

            Console.Out.WriteLine(ConsoleColor.Green, _display);
            Console.Out.Write("select>");
            string input = Console.ReadLine();
            while (input != "0" && input != "quit" && input != "q" && input != "exit")
            {
                if (input.Equals("cls", StringComparison.OrdinalIgnoreCase))
                {
                    Console.Clear();
                }
                int idx;
                if (int.TryParse(input, out idx))
                {
                    if (idx > 0 && idx <= list.Count)
                    {
                        Console.Clear();
                        Console.Out.WriteLine(ConsoleColor.DarkCyan, list[idx - 1] + " Running...");
                        list[idx - 1].Execute();
                        Console.Out.WriteLine(ConsoleColor.DarkCyan, list[idx - 1] + " Complete...");
                    }
                }
                Console.Out.WriteLine();
                lrTag("Select the use-case", "-", 20);
                Console.Out.WriteLine(ConsoleColor.Green, _display);
                Console.Out.Write("select>");
                input = Console.ReadLine();
            }
        }
    }

    /// <summary>
    /// The space
    /// </summary>
    private static readonly string SPACE = "  ";

    /// <summary>
    /// Lrs the tag.
    /// </summary>
    /// <param name="view">The view.</param>
    /// <param name="tag">The tag.</param>
    /// <param name="size">The size.</param>
    private static void lrTag(string view, string tag, int size)
    {
        StringBuilder sb = new StringBuilder();

        for (int i = 0; i < size; i++)
        {
            sb.Append(tag);
        }

        Console.Out.WriteLine(ConsoleColor.Yellow, sb + SPACE + view + SPACE + sb);
    }

    /// <summary>
    /// Writes the line.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="color">The color.</param>
    /// <param name="format">The format.</param>
    /// <param name="args">The arguments.</param>
    private static void WriteLine(this TextWriter writer,
        ConsoleColor color,
        string format,
        params object[] args)
    {
        Console.ForegroundColor = color;
        writer.WriteLine(format, args);
        Console.ResetColor();
    }

    /// <summary>
    /// Class ExecuteFunc.
    /// </summary>
    private class ExecuteFunc
    {
        private object _instance;

        private MethodInfo _method;

        private string _description;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExecuteFunc"/> class.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="method">The method.</param>
        /// <param name="description">The description.</param>
        public ExecuteFunc(object instance, MethodInfo method, string description = "")
        {
            _instance = instance;
            _method = method;

            if (string.IsNullOrEmpty(description))
            {
                _description = string.Concat("\t", instance.GetType().FullName, ".", method.Name);
            }
            else
            {
                _description = string.Concat("\t", instance.GetType().FullName, "." + method.Name,
                     "\t", description);
            }
        }

        /// <summary>
        /// Executes this instance.
        /// </summary>
        public void Execute()
        {
            _method.Invoke(_instance, null);
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            return _description;
        }
    }
}