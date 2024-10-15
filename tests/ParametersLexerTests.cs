
namespace SoftPatterns.Cli.Reader.Tests
{
    [TestClass]
    public class ParametersLexerTests
    {
        public enum ArgsStyle { Posix, Dos }

        [TestMethod]
        [DataRow("value", ArgsStyle.Posix, DisplayName = "Posix")]
        [DataRow("value", ArgsStyle.Dos, DisplayName = "Dos")]
        public void GetTokens_WithValue_ValueTokenIsReturned(string arg, ArgsStyle argsStyle)
        {
            // setup

            var lexer = CreateLexer(argsStyle);
            var substitutedToken = new ParameterToken(ParameterTokenType.None, 0, "substituted-token", 0, null);
            var argToken = new ParameterToken(ParameterTokenType.RawArg, 0, arg, 0, substitutedToken);
            var expectedToken = new ParameterToken(ParameterTokenType.Value, 0, "value", 0, argToken);
            var args = new[] { argToken.Value };

            // test

            var tokens = lexer.GetTokens(args, substitutedToken);

            Assert.IsTrue(tokens.TryGetAllItems(out var item));
            Assert_AreEqual(expectedToken, item); // a value token was parsed
        }

        [TestMethod]
        [DataRow("--name", 2, ArgsStyle.Posix, DisplayName = "Posix")]
        [DataRow("/name", 1, ArgsStyle.Dos, DisplayName = "Dos")]
        public void GetTokens_WithName_NameTokenIsReturned(string arg, int nameStart, ArgsStyle argsStyle)
        {
            // setup

            var lexer = CreateLexer(argsStyle);
            var substitutedToken = new ParameterToken(ParameterTokenType.None, 0, "substituted-token", 0, null);
            var argToken = new ParameterToken(ParameterTokenType.RawArg, 0, arg, 0, substitutedToken);
            var expectedToken = new ParameterToken(ParameterTokenType.Name, 0, "name", nameStart, argToken);
            var args = new[] { argToken.Value };

            // test

            var tokens = lexer.GetTokens(args, substitutedToken);

            Assert.IsTrue(tokens.TryGetAllItems(out var token));
            Assert_AreEqual(expectedToken, token); // a name token was parsed
        }

        [TestMethod]
        [DataRow("--name:value", 2, 7, ArgsStyle.Posix, DisplayName = "Posix With Column Separator")]
        [DataRow("--name=value", 2, 7, ArgsStyle.Posix, DisplayName = "Posix With Equal Separator")]
        [DataRow("--name value", 2, 7, ArgsStyle.Posix, DisplayName = "Posix With Space Separator")]
        [DataRow("/name:value", 1, 6, ArgsStyle.Dos, DisplayName = "Dos With Column Separator")]
        [DataRow("/name=value", 1, 6, ArgsStyle.Dos, DisplayName = "Dos With Equal Separator")]
        [DataRow("/name value", 1, 6, ArgsStyle.Dos, DisplayName = "Dos With Space Separator")]
        public void GetTokens_WithNameAndAttachedValue_AttachedValueTokenIsReturned(string arg, int nameStart, int valueStart, ArgsStyle argsStyle)
        {
            // setup

            var lexer = CreateLexer(argsStyle);
            var substitutedToken = new ParameterToken(ParameterTokenType.None, 0, "substituted-token", 0, null);
            var argToken = new ParameterToken(ParameterTokenType.RawArg, 0, arg, 0, substitutedToken);
            var expectedNameToken = new ParameterToken(ParameterTokenType.Name, 0, "name", nameStart, argToken);
            var expectedValueToken = new ParameterToken(ParameterTokenType.AttachedValue, 0, "value", valueStart, argToken);
            var args = new[] { argToken.Value };

            // test

            var tokens = lexer.GetTokens(args, substitutedToken);

            Assert.IsTrue(tokens.TryGetAllItems(out var nameToken, out var valueToken));
            Assert_AreEqual(expectedNameToken, nameToken); // a name token was parsed
            Assert_AreEqual(expectedValueToken, valueToken); // a value token was parsed
        }

        [TestMethod]
        public void GetTokens_WithNameGroup_NameTokensAreReturned()
        {
            // setup

            var lexer = ParametersLexer.Instance;
            var substitutedToken = new ParameterToken(ParameterTokenType.None, 0, "substituted-token", 0, null);
            var argToken = new ParameterToken(ParameterTokenType.RawArg, 0, "-abc", 0, substitutedToken);
            var expectedGroupToken = new ParameterToken(ParameterTokenType.NameGroup, 0, "abc", 1, argToken);
            var expectedNameToken1 = new ParameterToken(ParameterTokenType.ShortName, 0, "a", 0, expectedGroupToken);
            var expectedNameToken2 = new ParameterToken(ParameterTokenType.ShortName, 1, "b", 1, expectedGroupToken);
            var expectedNameToken3 = new ParameterToken(ParameterTokenType.ShortName, 2, "c", 2, expectedGroupToken);
            var args = new[] { argToken.Value };

            // test

            var tokens = lexer.GetTokens(args, substitutedToken);

            Assert.IsTrue(tokens.TryGetAllItems(out var token1, out var token2, out var token3)); // name tokens were parsed
            Assert_AreEqual(expectedNameToken1, token1);
            Assert_AreEqual(expectedNameToken2, token2);
            Assert_AreEqual(expectedNameToken3, token3);
        }

        [TestMethod]
        public void GetTokens_WithNameGroupAndAttachedValue_AttachedValueTokenIsReturned()
        {
            // setup

            var lexer = ParametersLexer.Instance;
            var substitutedToken = new ParameterToken(ParameterTokenType.None, 0, "substituted-token", 0, null);
            var argToken = new ParameterToken(ParameterTokenType.RawArg, 0, "-ab:value", 0, substitutedToken);
            var expectedGroupToken = new ParameterToken(ParameterTokenType.NameGroup, 0, "ab", 1, argToken);
            var expectedNameToken1 = new ParameterToken(ParameterTokenType.ShortName, 0, "a", 0, expectedGroupToken);
            var expectedNameToken2 = new ParameterToken(ParameterTokenType.ShortName, 1, "b", 1, expectedGroupToken);
            var expectedValueToken = new ParameterToken(ParameterTokenType.AttachedValue, 0, "value", 4, argToken);
            var args = new[] { argToken.Value };

            // test

            var tokens = lexer.GetTokens(args, substitutedToken);

            Assert.IsTrue(tokens.TryGetAllItems(out var nameToken1, out var nameToken2, out var valueToken)); // name tokens were parsed
            Assert_AreEqual(expectedNameToken1, nameToken1);
            Assert_AreEqual(expectedNameToken2, nameToken2);
            Assert_AreEqual(expectedValueToken, valueToken);
        }

        [TestMethod]
        public void GetTokens_WithEndOfOptionsFollowedByName_ValueTokenIsReturned()
        {
            // setup

            var lexer = ParametersLexer.Instance;
            var substitutedToken = new ParameterToken(ParameterTokenType.None, 0, "substituted-token", 0, null);
            var delimiterArgToken = new ParameterToken(ParameterTokenType.RawArg, 0, "--", 0, substitutedToken);
            var nameArgToken = new ParameterToken(ParameterTokenType.RawArg, 1, "--name", 0, substitutedToken);
            var expectedToken = new ParameterToken(ParameterTokenType.Value, 0, "--name", 0, nameArgToken);
            var args = new[] { delimiterArgToken.Value, nameArgToken.Value };

            // test

            var tokens = lexer.GetTokens(args, substitutedToken);

            Assert.IsTrue(tokens.TryGetAllItems(out var token));
            Assert_AreEqual(expectedToken, token); // a value token was parsed
        }

        private static void Assert_AreEqual(IParameterToken expectedToken, IParameterToken actualToken)
        {
            Assert.AreEqual(expectedToken.Type, actualToken.Type);
            Assert.AreEqual(expectedToken.Index, actualToken.Index);
            Assert.AreEqual(expectedToken.Value, actualToken.Value);
            Assert.AreEqual(expectedToken.ValueStartIndex, actualToken.ValueStartIndex);

            if (expectedToken.SubstitutedToken != null && actualToken.SubstitutedToken != null)
            {
                Assert_AreEqual(expectedToken.SubstitutedToken, actualToken.SubstitutedToken);
            }
            else if (expectedToken.SubstitutedToken != null)
            {
                Assert.Fail("SubstitutedToken is expected");
            }
            else if (actualToken.SubstitutedToken != null)
            {
                Assert.Fail("SubstitutedToken is not expected");
            }
        }

        private static IParametersLexer CreateLexer(ArgsStyle argsStyle)
        {
            return argsStyle switch
            {
                ArgsStyle.Posix => ParametersLexer.Instance,
                ArgsStyle.Dos => DosParametersLexer.Instance,
                _ => throw new NotSupportedException($"{nameof(ArgsStyle)} value \"{argsStyle}\" is not expected"),
            };
        }
    }
}
