namespace CompilerMapper.TestApp
{
    [MapperInterface]
    public interface IMapper
    {
        ProductDto Map(Product product);
    }
}
