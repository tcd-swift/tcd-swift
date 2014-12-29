var x: Int = 420;
var y: Int = 400 + 40 / 2
var z: Int = (10 + 200) * 2
z = z * 1

let c = ((400 + 40) / 2)

var s: String = "Hello World!"

var b: Bool = x == y

if (b) {
    var x = 100
}
else if x != y {
    var y = 100
}
else {
    var z = 100
}

while x == y {
  x = x + y
}

do {
 y = y + z
} while z == y


for var y = 0; y < 10; y = y + 1 {
    x = x + 1
}

switch x {
    case x:
        var x = 0 + y
    case y + 1:
        var x = 0 - y
    default:
        x = x + 1
}

func increment(x: Int) -> Int {
    return x + 1
}

func add(j: Int, k: Int) -> Int {
    return j + k
}

func check(b: Bool, i: Int, j: Int) -> Int {
    if b {
        return i
    } else {
        return j
    }
}

var q = increment(x)
q = add(x, 1)
q = add(1+1, 2*2)
var p = check(q == z, 1, 2)
