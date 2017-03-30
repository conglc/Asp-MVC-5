using MvcTemplate.Components.Extensions;
using Xunit;

namespace MvcTemplate.Tests.Unit.Components.Extensions
{
    public class JsTreeTests
    {
        #region JsTree()

        [Fact]
        public void JsTree_CreatesEmpty()
        {
            JsTree actual = new JsTree();

            Assert.Empty(actual.Nodes);
            Assert.Empty(actual.SelectedIds);
        }

        #endregion
    }
}
