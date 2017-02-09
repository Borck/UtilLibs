namespace Util.Collections {
  internal class LruMapItem<TK, TV> {
    public TK Key;
    public TV Value;

    public LruMapItem(TK k, TV v) {
      Key=k;
      Value=v;
    }
  }
}