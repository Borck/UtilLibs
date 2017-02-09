using System;

namespace Util.Exceptions {
  public static class SystemExt {
    public static Exception GetInnerMostException(this Exception e) {
      var result = e;
      while (result.InnerException!=null)
        result=result.InnerException;
      return result;
    }
  }
}