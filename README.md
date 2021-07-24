# Hotkey-Work-Logger

A minimalistic app to enable you to log the time you spend on tasks in the fastest way possible.

When started (preferably by hotkey), you are presented with a list of tasks (which can be easily changed) 
to choose from. Pressing the hotkey for a task appends a timestamped line to the log file and then 
closes the app.

This makes for quick and easy logging. The main disadvantage is having to remember to take the 1-2 
seconds it takes to execute the app every time you switch tasks or take a break;

## Usage

A console showing this is shown when the application is first started:

```console
Hotkey Work Logger
[1] Task 1
[2] Task 2
[3] Task 3
[M] Misc work
[B] Break
Choose a work item, enter a [c]ustom item, [e]dit the list or [v]iew the log file:
```

- When `[c]` is pressed, the user can enter the name of a work item once.
- When `[e]` is pressed, the `workitems.ini` file is opened in the default app (Notepad by default on Windows).
- When `[v]` is pressed, the `log.csv.txt` file is opened in the default app (Notepad by default on Windows).

### File: workitems.ini

This file exists in the same folder as the app executable, and it contains a key-value list of work items that 
will be shown when the app is opened. The contents are expected to be edited in a simple text editor like Notepad.

### File: log.csv.txt

This file exists in the same folder as the app executable, and it contains the actual log. 

After logging some tasks the contents looks somewhat like this:

```
2021-07-24 09:21:23;Task 1
2021-07-24 09:25:27;Misc work
2021-07-24 09:29:30;Break
```

The extension of this file is `.txt` to enable quick editing in a text editor, but the format is actually that of a semicolon (`;`) delimited CSV.

## Configuring a hotkey (Windows)

1. Create a shortcut to the executable.
2. Rightclick the executable and open Properties.
3. On the Shortcut tab, place the cursor in the Shortcut Key field.
4. Enter a key (combination), for example [CTRL]+[F12].
5. Press [OK].

* If shortcut keys work slow on your system, try disable the 'SysMain' service (previously called SuperFetch).
