"""
Simple Python test script for C# integration
Bu script C#'tan çağrılabilirliği test eder
"""

def hello_from_python(name="World"):
    """
    Basit test fonksiyonu
    """
    message = f"Hello from Python, {name}!"
    print(message)
    return message


def add_numbers(a, b):
    """
    İki sayıyı toplar - basit veri aktarımı testi
    """
    result = a + b
    print(f"Python: {a} + {b} = {result}")
    return result


if __name__ == "__main__":
    # Standalone test
    hello_from_python("AlgoTrade")
    add_numbers(10, 20)
