def printOutputPretty(dfs):
    from IPython.display import display, HTML

    output = f"""
    <div>
        <table>
            <tr> 
    """
    for df in dfs:
        output += f'<th style="text-align:center"> {df.name} </th>'
    output += '</tr><tr>'
    
    for df in dfs:
        output += f'<td> {df.head().to_html()}</td>'
    output += '</tr> </table> </diV>'
    
    display(HTML(output))