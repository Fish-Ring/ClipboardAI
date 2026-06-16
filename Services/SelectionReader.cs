using System.Windows.Automation;

namespace ClipboardAI.Services;

public static class SelectionReader
{
    public static string? GetSelectedText()
    {
        try
        {
            var focused = AutomationElement.FocusedElement;
            if (focused == null) return null;

            if (focused.TryGetCurrentPattern(TextPattern.Pattern, out object? patternObj) && patternObj is TextPattern tp)
            {
                var selection = tp.GetSelection();
                if (selection.Length > 0)
                    return selection[0].GetText(-1);
            }

            if (focused.TryGetCurrentPattern(ValuePattern.Pattern, out object? valueObj) && valueObj is ValuePattern vp)
            {
                var text = vp.Current.Value;
                if (!string.IsNullOrEmpty(text)) return text;
            }

            var name = focused.Current.Name;
            return string.IsNullOrEmpty(name) ? null : name;
        }
        catch
        {
            return null;
        }
    }
}
