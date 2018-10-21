# c#-awr-class : advance quran tajwid processor
awr is an advance wordprocessor/rulemaking for the Arabic language especially used for apply/showing Quran tajwid rules in real-time
online demo :


[rokhan](http://rokhan.ir)

[php version](https://github.com/pfndesign/php-awr-class)

## how is this works :

with this class, you can make rules based on characters and their position to each other, tag them an show them with different colors

## usage :

```c#
                    using rokhan.inc;
                    awr_process awr = new awr_process(string);
                    awr.Process_event += new processHandler(awr.Filter_qalqala);
                    awr.Process_event += new processHandler(awr.Filter_ghunna);
                    awr.Process_event += new processHandler(awr.Filter_lqlab);
                    awr.Process_event += new processHandler(awr.Filter_ikhfaa);
                    awr.Process_event += new processHandler(awr.Filter_idgham);
                    awr.Process_event += new processHandler(awr.Filter_idgham_without_ghunna);
                    awr.Process_event += new processHandler(awr.Filter_maddah);
                    awr.Process();
                    awr.Reorder();
                    awr.Render(rule_ayehbox);
```

## how to use :

### setup :
```c#
awr_process awr = new awr_process(string);
```

### register rules :
there are 7 rules that I created as part of tajwid rules

1. filter_qalqala
2. filter_ghunna
3. filter_lqlab
4. filter_ikhfaa
5. filter_idgham
6. filter_idgham_without_ghunna
7. filter_maddah

in order for filters/rules to work they must be registered
you can register filters/rules with Process_event

```c#
awr.Process_event
```

for example

```c#
awr.Process_event += new processHandler(awr.Filter_qalqala);
```

### finishing up : 

in order to fillters to work you have to call process() function . it will run every filter for every charecter in the text

```c#
awr.Process();
```

after that reorder() must be called to restore characters original forms

```c#
awr.Reorder();
```
### rendering : 

you can render the text in RichTextBox
to render the final results you have to call render() function with RichTextBox
  
```c#
awr.Render(rule_ayehbox);
```
### aditional functions

appending text with color to RichTextBox

```c#
AppendText(RichTextBox box, SolidColorBrush color, string text)
```
### changing colors

you can add colors to public method colors or edit the default colors


```c#
public Dictionary<String, SolidColorBrush> colors = new Dictionary<string, SolidColorBrush>{
                {"none" , (SolidColorBrush)(new BrushConverter().ConvertFrom("#000000")) },
                {"chunna" , (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF6600")) },
                {"ikhfaa" , (SolidColorBrush)(new BrushConverter().ConvertFrom("#CC0000")) },
                {"qalqala" , (SolidColorBrush)(new BrushConverter().ConvertFrom("#00CC00")) },
                {"lqlab" , (SolidColorBrush)(new BrushConverter().ConvertFrom("#6699FF")) },
                {"idghamwg" , (SolidColorBrush)(new BrushConverter().ConvertFrom("#BBBBBB")) },
                {"idgham" , (SolidColorBrush)(new BrushConverter().ConvertFrom("#9900CC")) },
                {"maddah" , (SolidColorBrush)(new BrushConverter().ConvertFrom("#34495e")) },
            };
```

