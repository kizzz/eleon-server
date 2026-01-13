using Xunit; using __NS__.__MOD_NAME__.Application; public class ExampleTests{ [Fact] public void Adds(){ var s=new ExampleDomainService(); Assert.Equal(3,s.Add(1,2)); } }
