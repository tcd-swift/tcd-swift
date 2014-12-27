{STORE, T$0, 420}
{STORE, x, T$0}
{STORE, T$1, 400}
{STORE, T$2, 40}
{STORE, T$3, 2}
{DIV, T$4, T$2, T$3}
{ADD, T$5, T$1, T$4}
{STORE, y, T$5}
{STORE, T$6, 20}
{STORE, T$7, 200}
{STORE, T$8, 2}
{MUL, T$9, T$7, T$8}
{ADD, T$10, T$6, T$9}
{STORE, z, T$10}
{STORE, T$11, 210}
{STORE, w, T$11}
{STORE, T$12, 2}
{MUL, T$13, w, T$12}
{STORE, w, T$13}
{STORE, T$14, "Hello World!"}
{STORE, s, T$14}
{EQU, T$15, x, y}
{STORE, b, T$15}
{JMPF, b, L$0}
{STORE, T$16, "if (b)"}
{STORE, s1, T$16}
{JMP, L$1}
{LABEL, L$0}
{JMPF, b, L$2}
{STORE, T$17, "else if b"}
{STORE, s2, T$17}
{JMP, L$3}
{LABEL, L$2}
{STORE, T$18, "else"}
{STORE, s3, T$18}
{LABEL, L$3}
{LABEL, L$1}
