func example1(u: Int, v: Int) -> Int {
  if((u > 0) & (v > 0)) {
    do {
      if(u < v) {
        var temp: Int = u
        u = v
        v = temp
      }
      u = u - v
    } while u != v
  }
  return u;
}

var u: Int = 10;
var v: Int = 42;
var result = example1(u, v)