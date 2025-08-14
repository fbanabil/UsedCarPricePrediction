import pandas as pd
import json
import sys
from car_price_inference import CarPricePredictor

def main():
    try:
        input_data = sys.stdin.read()
        if not input_data.strip():
            sys.exit(1)
        
        car_data = json.loads(input_data)

        sample_data = pd.DataFrame([car_data])
        
        predictor = CarPricePredictor()
        
        predicted_price = predictor.predict(sample_data)
        
        result = {
            "predicted_price": float(predicted_price[0]),
            "formatted_price": f"${predicted_price[0]:,.2f}"
        }
        print(json.dumps(result, indent=2))
        
    except json.JSONDecodeError:
        sys.exit(1)
    except KeyError as e:
        sys.exit(1)
    except Exception as e:
        sys.exit(1)

if __name__ == "__main__":
    main()