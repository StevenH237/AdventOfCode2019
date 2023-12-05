This is a free-to-use template for Advent of Code programs! All I ask is that you leave the LICENSE file intact (though you may rename it and apply your own license to the resulting repository).

The template is set up so that you can jump straight into coding the solutions without worrying about the boilerplate to run it. There is a little bit of fine print to that, though:

1. For each day, you will need to create a public class called `DayXX`, replacing `XX` with the day number (using a single digit for 1 through 9). This class must have public methods named `Part1` and `Part2`, which accept a `string` and `StreamReader` as their parameters, and return a `string`. The input `string` is a filename, and the `StreamReader` will contain file inputs. For the example/test files, it will have already read the first two lines (containing what that answer should be for each part) and be ready to read the rest of the files.
2. Your puzzle input for each day should be located at `(workspace root)/data/dayXX/input.txt`. It should be unmodified from what's given on the site - the `StreamReader` will **not** skip any lines on the actual input!
3. Example inputs for each day should be located at `(root)/data/dayXX/exampleZZ.txt`, where `ZZ` can be replaced with anything or nothing at all. The first line of the file should be the expected output if that input is given to part 1, and the second line should be the expected output if that input is given to part 2. Leave a line blank if that input shouldn't be given to that part.
4. The `data` folder is `.gitignore`d as the creator of Advent of Code [requests](https://twitter.com/ericwastl/status/1465805354214830081) that you not share inputs. If you're committing to a private repository, feel free to take out that gitignore. Otherwise, you might want to look into submodules!

If you've used my [original template](https://github.com/StevenH237/AdventOfCodeTemplate), this is **entirely rewritten** and not compatible with that one. It's updated to .NET 8 and has nicer testing capabilities, imo.

"AdventOfCodeTemplate2" doesn't exist. Don't worry about it. ☺