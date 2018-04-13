//
// Based on article "BigArray<T>, getting around the 2GB array size limit" by Josh Wil (MSDN).
// https://blogs.msdn.microsoft.com/joshwil/2005/08/10/bigarrayt-getting-around-the-2gb-array-size-limit/
//

namespace fVis.Utils
{
    public class BigArray<T>
    {
        // These need to be const so that the getter/setter get inlined by the JIT into
        // calling methods just like with a real array to have any chance of meeting our
        // performance goals.

        // BLOCK_SIZE must be a power of 2, and we want it to be big enough that we allocate
        // blocks in the large object heap so that they don’t move.
        private const uint BLOCK_SIZE = 524288;
        private const int BLOCK_SIZE_LOG2 = 19;

        // Don’t use a multi-dimensional array here because then we can’t right size the last
        // block and we have to do range checking on our own and since there will then be 
        // exception throwing in our code there is a good chance that the JIT won’t inline.
        private readonly T[][] elements;
        private readonly ulong length;

        public ulong Length
        {
            get
            {
                return length;
            }
        }

        public T this[ulong elementNumber]
        {
            get
            {
                int blockNum = (int)(elementNumber >> BLOCK_SIZE_LOG2);
                int elementNumberInBlock = (int)(elementNumber & (BLOCK_SIZE - 1));
                return elements[blockNum][elementNumberInBlock];
            }
            set
            {
                int blockNum = (int)(elementNumber >> BLOCK_SIZE_LOG2);
                int elementNumberInBlock = (int)(elementNumber & (BLOCK_SIZE - 1));
                elements[blockNum][elementNumberInBlock] = value;
            }
        }

        public BigArray(ulong size)
        {
            uint numBlocks = (uint)(size / BLOCK_SIZE);
            if ((numBlocks * BLOCK_SIZE) < size) {
                numBlocks += 1;
            }

            length = size;
            elements = new T[numBlocks][];
            for (int i = 0; i < (numBlocks - 1); i++) {
                elements[i] = new T[BLOCK_SIZE];
            }

            uint numElementsInLastBlock = (uint)(size - ((numBlocks - 1) * BLOCK_SIZE));
            elements[numBlocks - 1] = new T[numElementsInLastBlock];
        }
    }
}