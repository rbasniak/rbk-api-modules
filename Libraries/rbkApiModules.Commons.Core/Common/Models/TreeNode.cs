namespace rbkApiModules.Commons.Core;

public class TreeNode<T>
{
    public string Label { get; set; }
    public object Data { get; set; }
    public string Icon { get; set; }
    public string ExpandedIcon { get; set; }
    public string CollapsedIcon { get; set; }
    public bool Leaf { get; set; }
    public string Type { get; set; }
    public string Style { get; set; }
    public string StyleClass { get; set; }
    public bool Draggable { get; set; }
    public bool Droppable { get; set; }
    public bool Selectable { get; set; }
    public string Key { get; set; }
    public bool Expanded { get; set; }
    public List<T> Children { get; set; }
}

public class TreeNode 
{
    public string Label { get; set; }
    public object Data { get; set; }
    public string Icon { get; set; }
    public string ExpandedIcon { get; set; }
    public string CollapsedIcon { get; set; }
    public bool Leaf { get; set; }
    public string Type { get; set; }
    public string Style { get; set; }
    public string StyleClass { get; set; }
    public bool Draggable { get; set; }
    public bool Droppable { get; set; }
    public bool Selectable { get; set; }
    public string Key { get; set; }
    public bool Expanded { get; set; }
    public List<TreeNode> Children { get; set; }

    public override string ToString()
    {
        return Label;
    }
}