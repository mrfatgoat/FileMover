# FileMover
FileMover is a console application that makes moving and organizing lots of files simple.

### Getting Started
In order for FileMover to work, your destination directory must contain a configuration file called ```filemover.json```.
Create a sample configuration file by using the ```/init``` command line parameter like so: ```FileMover /init c:\myfiles\```

```c:\myfiles\``` will contain ```filemover.json``` which looks something like this:
```
{
  "Folders": {
    "Pictures": "Pictures",
    ...
  },
  "Destinations": {
    "jpg": "Folders:Pictures",
    "png": "Folders:Pictures",
    "gif": "Folders:Pictures",
    ...
  }
}
```

Once the configuration file is in place, you can move files from a source folder ```<src>``` into folders nested within ```c:\myfiles\``` using the following command line parameters: ```FileMover <src> c:\myfiles\```. Note that ```<src>``` can be a single file or a folder containing files to move.

### Configuration
The configuration file is easy to interpret and modify to your liking.  The ```"Folders"``` node defines all of the destination folders in which you intend to organize files, relative to the destination directory.
The ```"Destinations"``` node defines mappings between file extensions and destinations for files matching those extensions.
For example, using the sample configuration, all ```*.jpg, *.png, *.gif``` files will end up in ```c:\myfiles\Pictures\```.

### FAQ
**Why don't some of my files get moved?**

- *Ensure the files' extensions are listed in ```"Destinations"``` and that the folder they point to a folder defined in ```"Folders"```*

**Will FileMover overwrite a files in my destination?**

- *Nope!*

### Making FileMover Convenient to Use (optional)
My preferred way of interacting with FileMover is to right-click a folder and send its entire contents to a designated destination. To achieve this, there are a few more working parts.

**Windows 7+** (*maybe older versions too, but really...why?*)

First, open the shell shortcuts folder:

```Start > run > shell:sendto``` or ```Start > run > C:\Users\<username>\AppData\Roaming\Microsoft\Windows\SendTo```

Next, create a new batch file named after your destination such as ```My Files.bat```. Add the following lines to your batch file:

```
@FOR %%A IN (%*) DO (
  c:\path\to\FileMover %%A "c:\myfiles\"
)
```

Now, whenever you right-click on a file or folder in Windows, you will be able to see your batch file under the "Send to" context menu.
Whatever you send to your batch file will be the ```<src>``` parameter to FileMover and will be sent to ```c:\myfiles\```.

### Planned Features
- Move files from ```<src>``` recursively
- Pattern matching in addition to file extensions
