class Program
{
    static void Main()
    {
        var forest = new List<Tree>();

        forest.Add(new Tree(10, 20, TreeFactory.GetTreeType("松树", "绿色", "粗糙")));
        forest.Add(new Tree(15, 25, TreeFactory.GetTreeType("松树", "绿色", "粗糙")));
        forest.Add(new Tree(30, 40, TreeFactory.GetTreeType("橡树", "深绿色", "光滑")));

        foreach (var tree in forest)
        {
            tree.Draw();
        }
    }
}
