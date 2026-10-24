namespace Assets.Scripts.Utils
{
    public static class IntExtensions
    {
        public static int Seeded(this int baseSeed, int id)
        {
            // Això permet que no hi hagi presents errors de stack Overflow
            unchecked
            {
                int hash = baseSeed;

                // 1. Barrejar l'id dintre del hash (com afegir informació)
                hash = hash * 31 + id;
                // *31 es un multiplicador clàssic (nombre primer). 
                // És equivalent a "hash = hash << 5 - hash" (molt ràpid.
                // Això incorpora l'id al valor actual.

                // 2. Primer "scramble" (barreja de bits)
                hash ^= (hash >> 15);
                // ^ = XOR (exclusive or)
                // >> 15 desplaça els bits 15 posicions cap a la dreta.
                // XOR amb el seu propi desplaçament barreja els bits alts amb els baixos.
                // Això ajuda a que canvis petits en l'input afectin a molts bits.

                // 3. Multiplicació amb constant màgica
                hash *= (int)0x85ebca6b;
                // 0x85ebca6b es una constant de 32 bits utilitzada en MurmurHash3.
                // Multiplicar per aquesta constant.
                // difon (avalanche effect) els cambis de bit per tot el nombre.

                // 4. Segon scramble
                hash ^= (hash >> 13);
                // Similar al pas 2, pero amb un desplaçament de 13 bits.
                // Segueix barrejant els alts amb els baixos

                // 5. Segona multiplicació amb una altra constant
                hash *= (int)0xc2b2ae35;
                // Una altre constant màgica de MurmurHash3. 
                // Una altra ronda de difusió forta.

                // 6. Tercer y últim scramble
                hash ^= (hash >> 16);
                // Desplaçament de 16 bits.
                // Això és la fase final típica per barrejar bé els 32 bits.

                return hash;
            }
        }
    }
}