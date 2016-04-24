namespace Nuspecr
{
    using CommandLine;

    class Program
    {
        static int Main(string[] args)
        {
            return Parser.Default.ParseArguments<SpecOptions>(args)
                .MapResult(
                (SpecOptions opts) => RunSpecAndReturnExitCode(opts),
                errs => 1);
        }

        private static int RunSpecAndReturnExitCode(SpecOptions options)
        {
            var specr = new Specr();

            foreach (var input in options.InputFiles)
            {
                specr.CreateNuspec(input, options.VersionSource);
            }

            return 0;
        }
    }
}
