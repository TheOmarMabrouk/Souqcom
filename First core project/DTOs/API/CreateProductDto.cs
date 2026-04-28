namespace First_core_project.DTOs.API
{
    public class ProductCreateDto
    {
        public string Name { get; set; }
        public string Discription { get; set; }
        public decimal Price { get; set; }
        public int Catid { get; set; }
        public string Photo { get; set; }
        public string SupplierName { get; set; }
       
    }
}
