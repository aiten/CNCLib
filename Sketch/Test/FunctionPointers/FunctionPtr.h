
class CFunctionPtr
{

 public:
  
typedef void(*TestFunction)(void);

struct STest
{
  TestFunction fnc;
  int hallo;
};

static STest testarr[2]  ;

static void Test();

};
