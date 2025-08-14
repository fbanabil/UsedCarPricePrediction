# car_price_inference.py
import pandas as pd
import numpy as np
import joblib
import warnings
from scipy.stats import boxcox
from scipy.sparse import hstack, csr_matrix
from sklearn.preprocessing import PowerTransformer
import os

warnings.filterwarnings('ignore')

class CarPricePredictor:
    current_dir = os.getcwd()
    def __init__(self, model_path=current_dir+"/ML_Models"):
        self.model_path = model_path
        self.load_models()
    
    def load_models(self):
        try:
            self.best_xgb = joblib.load(f'{self.model_path}/best_xgb_model.pkl')
            self.best_lgb = joblib.load(f'{self.model_path}/best_lgb_model.pkl')
            self.best_cat = joblib.load(f'{self.model_path}/best_cat_model.pkl')
            self.meta_model = joblib.load(f'{self.model_path}/meta_model.pkl')

            self.tfidf = joblib.load(f'{self.model_path}/tfidf_vectorizer.pkl')
            self.encoder = joblib.load(f'{self.model_path}/target_encoder.pkl')
            self.preprocessor = joblib.load(f'{self.model_path}/column_preprocessor.pkl')
            self.kmeans = joblib.load(f'{self.model_path}/geo_kmeans.pkl')
            self.pt_dict = joblib.load(f'{self.model_path}/power_transformers.pkl')
            self.cat_imputer = joblib.load(f'{self.model_path}/cat_imputer.pkl')
            self.num_imputer = joblib.load(f'{self.model_path}/num_imputer.pkl')
            self.lambda_ = joblib.load(f'{self.model_path}/boxcox_lambda.pkl')
            
        except Exception as e:
            print(f"❌ Error loading models: {e}")
            raise
    
    def preprocess_input(self, data):
        df = data.copy()
        df['car_age'] = 2025 - df['year']
        df['mileage_per_year'] = df['odometer'] / (df['car_age'] + 1)
        df['odometer_log'] = np.log1p(df['odometer'])
        df['lat_long_interaction'] = df['lat'] * df['long']

        num_cols = ['odometer', 'car_age', 'mileage_per_year', 'lat', 'long', 'lat_long_interaction']
        for col in num_cols:
            if col in self.pt_dict:
                df[col] = df[col].clip(lower=0.1) 
                df[f'boxcox_{col}'] = self.pt_dict[col].transform(df[[col]])
        
        num_cols = [f'boxcox_{col}' for col in num_cols] + ['year', 'odometer_log']
        
        df['age_squared'] = df['car_age'] ** 2
        df['odometer_squared'] = df['odometer'] ** 2
        num_cols.extend(['age_squared', 'odometer_squared'])
        
        df['geo_cluster'] = pd.Series(self.kmeans.predict(df[['lat', 'long']]), 
                                     index=df.index).astype('category')
        
        df['brand_tier'] = df['manufacturer'].apply(
            lambda x: 'luxury' if x in ['bmw','mercedes-benz','audi','lexus','porsche','jaguar','land rover','tesla']
            else 'mid' if x in ['toyota','honda','nissan','subaru','mazda','volkswagen','hyundai','kia']
            else 'economy'
        ).astype('category')
        

        df['make_model'] = (df['manufacturer'].astype(str) + '_' + df['model'].astype(str)).astype('category')

        features = num_cols + [
            'manufacturer', 'model', 'condition', 'cylinders', 'fuel', 'title_status',
            'transmission', 'drive', 'type', 'geo_cluster', 'brand_tier',
            'make_model', 'description'
        ]
        
        cat_cols = ['manufacturer', 'condition', 'cylinders', 'fuel',
                    'title_status', 'transmission', 'drive', 'type',
                    'geo_cluster', 'brand_tier', 'make_model']
        
        df[cat_cols] = self.cat_imputer.transform(df[cat_cols])
        df[num_cols] = self.num_imputer.transform(df[num_cols])
        
        X = df[features].copy()
        
        X['model_encoded'] = self.encoder.transform(X['model'])
        
        tfidf_matrix = self.tfidf.transform(X['description'].fillna(''))
        
        X = X.drop(['description','make_model','model'], axis=1, errors='ignore')
        
        X_processed = self.preprocessor.transform(X)
        X_processed = csr_matrix(X_processed)
        
        X_final = hstack([X_processed, tfidf_matrix], format='csr')
        
        return X_final
    
    def predict(self, data):
        X_processed = self.preprocess_input(data)
        
        pred_xgb = self.best_xgb.predict(X_processed)
        pred_lgb = self.best_lgb.predict(X_processed)
        pred_cat = self.best_cat.predict(X_processed)
        
        meta_features = np.vstack([pred_xgb, pred_lgb, pred_cat]).T
        
        pred_boxcox = self.meta_model.predict(meta_features)
        
        pred_boxcox = np.clip(pred_boxcox, -1e5, 1e5)
        pred_price = np.where(self.lambda_ == 0, 
                             np.exp(pred_boxcox), 
                             np.power(np.maximum(pred_boxcox * self.lambda_ + 1, 1e-9), 
                                     1 / self.lambda_))
        pred_price = np.clip(pred_price, 0, 1e6)
        
        return pred_price
