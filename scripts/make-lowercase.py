import pandas as pd

# Load the JSONL file into a DataFrame
input_file_path = "cmd_mapper_training_data_v5_validation.jsonl"
output_file_path = "cmd_mapper_training_data_v5_validation_lowercase.jsonl"
df = pd.read_json(input_file_path, lines=True)

# Convert text columns to lowercase
df['prompt'] = df['prompt'].str.lower()
df['completion'] = df['completion'].str.lower()

# Save the transformed data to a new JSONL file
df.to_json(output_file_path, orient='records', lines=True)

print(f"Data saved to {output_file_path}")
