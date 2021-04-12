using System;

namespace CompilerMapper.TestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            IMapper mapper = new Mapper();

            var produtDto = mapper.Map(new Product { Id = 10 });
            Console.WriteLine(produtDto.Id);
        }
    }
}
