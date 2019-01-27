# MarkDownConsole
Unified MarkDown Console for HTML and Shell Console

## The Idea?
In my current project we have lots of command logic that output calculation reports to the console. As the functionaliy matures, or reaches a larger audience, the it gets ported to the web.

Would it not be great if the console and html versions share the same interface and rendering abilities:
- Console Renders Markdown with ASCII-art and Console standard colours (Find closest Color -< ConsoleColour)
- Html Renders to a virtual console (thread-safe) and renders HTML Markdown

Interfaces:
 - ISimpleTextWriter ( Write + WriteLine )
 - IVirtualConsole : ISimpelTextWriter
    - Token
    - Title
    - Width
    - Height
    
Features:
- Markdown is the common format (Rendering to Console or HTML)
- Threadsafe for the web (many IVirtualConsoles at once)
- Easy inline colours "Hello ^red^World^default^" or string.Format("Hello {0}World{1}", Color.Red, Color.Default)


## Reserch / Related projects
- https://www.reddit.com/r/commandline/comments/3xgd2w/good_markdown_viewer_in_terminal/
- https://meta.stackexchange.com/questions/36833/markdown-console-application
- https://github.com/kneufeld/consolemd
- https://stackoverflow.com/questions/23904274/is-there-a-way-to-get-colored-text-in-github-flavored-markdown/41043795
