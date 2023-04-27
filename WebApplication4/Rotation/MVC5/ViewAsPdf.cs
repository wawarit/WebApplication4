namespace Rotation.MVC5
{
    internal class ViewAsPdf
    {
        public string ViewName { get; set; }
        public bool IsGrayScale { get; set; }
        public object PageSize { get; set; }
        public object PageOrientation { get; set; }
        public string CustomSwitches { get; set; }
        public int Model { get; set; }
        public string FileName { get; set; }
    }
}