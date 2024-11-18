#!/usr/bin/python3

import os
from pathlib import Path
from datetime import datetime

def print_and_save_structure(startpath, output_file, exclude_dirs=None, exclude_files=None):
    if exclude_dirs is None:
        exclude_dirs = {'.git', '__pycache__', '.idea'}
    if exclude_files is None:
        exclude_files = {'.gitignore', '.DS_Store'}
    
    output_lines = []
    output_lines.append("Scripts folder structure:")
    output_lines.append(f"Generated on: {datetime.now().strftime('%Y-%m-%d %H:%M:%S')}\n")
        
    for root, dirs, files in os.walk(startpath):
        dirs[:] = [d for d in dirs if d not in exclude_dirs]
        
        level = root.replace(startpath, '').count(os.sep)
        indent = '│   ' * level
        folder_line = f'{indent}└── {os.path.basename(root)}/'
        print(folder_line)
        output_lines.append(folder_line)
        
        subindent = '│   ' * (level + 1)
        for f in sorted(files):
            if f not in exclude_files and f.endswith('.cs'):
                file_line = f'{subindent}└── {f}'
                print(file_line)
                output_lines.append(file_line)

    # Force write to Assets folder
    output_path = os.path.join(os.path.dirname(startpath), "structure.txt")
    try:
        with open(output_path, 'w') as f:
            f.write('\n'.join(output_lines))
        print(f"\nStructure saved to: {output_path}")
    except Exception as e:
        print(f"\nError saving file: {e}")
        # Try alternative location
        desktop_path = os.path.expanduser("~/Desktop/unity_structure.txt")
        try:
            with open(desktop_path, 'w') as f:
                f.write('\n'.join(output_lines))
            print(f"\nStructure saved to desktop instead: {desktop_path}")
        except Exception as e2:
            print(f"\nError saving to desktop: {e2}")

if __name__ == "__main__":
    try:
        scripts_dir = os.path.join(os.path.dirname(__file__), "Scripts")
        
        print(f"Scripts directory: {scripts_dir}")
        
        if os.path.exists(scripts_dir):
            print("\nStarting structure analysis...")
            print_and_save_structure(scripts_dir, "")
        else:
            print(f"\nERROR: Could not find Scripts directory at: {scripts_dir}")
            
    except Exception as e:
        print(f"Error running script: {e}")
        
    input("\nPress Enter to exit...")